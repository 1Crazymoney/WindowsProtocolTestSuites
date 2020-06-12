// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter
{
    /// <summary>
    /// WspAdapter class implements the methods of the IWspAdapter interface.
    /// </summary>
    public class WspAdapter : ManagedAdapterBase, IWspAdapter
    {
        #region Fields

        //Platform version
        private SkuOsVersion _platform;

        /// <summary>
        /// startingIndex define the offset of the massage
        /// </summary>
        int startingIndexconnect;

        /// <summary>
        /// ITestSite to access the configurable data and requirement logging 
        /// </summary>
        public ITestSite wspTestSite = null;

        /// <summary>
        /// Sends WSP-Messages to the Server for negative scenarios
        /// (To validate messages with out sending the pre-requisite
        /// ConnectIn message)
        /// </summary>
        public WspClient defaultClient = null;
        /// <summary>
        /// Sends WSP-Messages to the Server for negative scenarios
        /// (To validate messages with out sending the pre-requisite
        /// ConnectIn message)
        /// </summary>
        RequestSender defaultSender = null;
        /// <summary>
        /// MessageBuilder builds client requests
        /// </summary>
        public MessageBuilder builder = null;
        /// <summary>
        /// Validates Server responses
        /// </summary>
        public MessageValidator validator = null;
        /// <summary>
        /// Maps list of connected clients
        /// </summary>
        Dictionary<string, WspClient> connectedClients = null;
        /// <summary>
        /// Maps a cursor corresponding to a client
        /// </summary>
        Dictionary<string, uint> cursorMap
            = new Dictionary<string, uint>();
        /// <summary>
        /// Name of the Server hosting Windows Search Service
        /// </summary>
        string serverMachineName = null;
        /// <summary>
        /// Path of the PIPE (Communication channel)
        /// </summary>
        string pipePath = null;
        /// <summary>
        /// Number of bytes read from the server for a 
        /// given request message
        /// </summary>
        int bytesRead = -1;
        /// <summary>
        /// Name of the connected Client
        /// </summary>
        public string clientMachineName = null;
        /// <summary>
        /// Catalog Name to Query
        /// </summary>
        public string catalogName = null;
        /// <summary>
        /// Name of the connected username
        /// </summary>
        string userName = null;
        /// <summary>
        /// Determines whether the client is connected
        /// </summary>
        bool isClientConnected = false;
        /// <summary>
        /// CbReserve Field of RowsIn message
        /// </summary>
        uint rowsInReserve = 0;
        /// <summary>
        /// Client Base Field of RowsIn message
        /// </summary>
        uint rowsInClientBase = 0;
        /// <summary>
        /// TableColumns to be queried
        /// </summary>
        TableColumn[] tableColumns = null;
        /// <summary>
        /// WorkId of the current row
        /// </summary>
        uint lastDocumentWorkId = 0;
        /// <summary>
        /// Language locale
        /// </summary>
        string locale = null;
        /// <summary>
        /// Client version
        /// </summary>
        public uint clientVersion = 0;

        /// <summary>
        /// Send an invalid _msg value in the message header
        /// </summary>
        public bool sendInvalidMsg = false;

        /// <summary>
        /// Send an invalid _status value in the message header.
        /// </summary>
        public bool sendInvalidStatus = false;

        /// <summary>
        /// Send an invalid _ulChecksum value in the message header. 
        /// </summary>
        public bool sendInvalidUlChecksum = false;

        /// <summary>
        /// Calculate the corret checksum of the message.
        /// </summary>
        public bool calculateChecksum = true;

        public CPMSetBindingsIn setBindingsIn;
        #endregion

        #region Initialize & Cleanup
        /// <summary>
        /// Initialises the fields of WspAdapter
        /// </summary>
        /// <param name="testSite">testSite</param>
        public override void Initialize(ITestSite testSite)
        {
            base.Initialize(testSite);
            wspTestSite = testSite;
            wspTestSite = TestClassBase.BaseTestSite;
            connectedClients = new Dictionary<string, WspClient>();
            wspTestSite.DefaultProtocolDocShortName = "MS-WSP";
            validator = new MessageValidator(wspTestSite);
            serverMachineName
                = wspTestSite.Properties.Get("ServerComputerName");
            clientMachineName = wspTestSite.Properties.Get("ClientName");
            catalogName = wspTestSite.Properties.Get("CatalogName");
            userName = wspTestSite.Properties.Get("UserName");
            pipePath
                = string.Format(@"\\{0}\\pipe\MSFTEWDS", serverMachineName);
            clientVersion = (uint)Convert.ToUInt32
                (wspTestSite.Properties["ClientVersion"]);

            defaultSender = new RequestSender(pipePath);

            defaultClient = new WspClient();

            defaultClient.sender = defaultSender;

            var parameter = InitializeParameter();
            builder = new MessageBuilder(parameter);

            isClientConnected = false;
        }

        private MessageBuilderParameter InitializeParameter()
        {
            char[] delimiter = new char[] { ',' };

            var parameter = new MessageBuilderParameter();

            parameter.PropertySet_One_DBProperties = wspTestSite.Properties.Get("PropertySet_One_DBProperties").Split(delimiter);

            parameter.PropertySet_Two_DBProperties = wspTestSite.Properties.Get("PropertySet_Two_DBProperties").Split(delimiter);

            parameter.Array_PropertySet_One_Guid = new Guid(wspTestSite.Properties.Get("Array_PropertySet_One_Guid"));

            parameter.Array_PropertySet_One_DBProperties = wspTestSite.Properties.Get("Array_PropertySet_One_DBProperties").Split(delimiter);

            parameter.Array_PropertySet_Two_Guid = new Guid(wspTestSite.Properties.Get("Array_PropertySet_Two_Guid"));

            parameter.Array_PropertySet_Two_DBProperties = wspTestSite.Properties.Get("Array_PropertySet_Two_DBProperties").Split(delimiter);

            parameter.Array_PropertySet_Three_Guid = new Guid(wspTestSite.Properties.Get("Array_PropertySet_Three_Guid"));

            parameter.Array_PropertySet_Three_DBProperties = wspTestSite.Properties.Get("Array_PropertySet_Three_DBProperties").Split(delimiter);

            parameter.Array_PropertySet_Four_Guid = new Guid(wspTestSite.Properties.Get("Array_PropertySet_Four_Guid"));

            parameter.Array_PropertySet_Four_DBProperties = wspTestSite.Properties.Get("Array_PropertySet_Four_DBProperties").Split(delimiter);

            parameter.EachRowSize = MessageBuilder.rowWidth;

            parameter.EType = UInt32.Parse(wspTestSite.Properties.Get("EType"));

            parameter.BufferSize = UInt32.Parse(wspTestSite.Properties.Get("BufferSize"));

            parameter.LCID_VALUE = UInt32.Parse(wspTestSite.Properties.Get("LCID_VALUE"));

            parameter.ClientBase = UInt32.Parse(wspTestSite.Properties.Get("ClientBase"));

            parameter.RowsToTransfer = UInt32.Parse(wspTestSite.Properties.Get("RowsToTransfer"));

            return parameter;
        }

        /// <summary>
        /// Closes all the communication Pipe and deallocate the resources used
        /// </summary>
        public override void Reset()
        {
            //defaultSender.handle.Close();
            foreach (var client in connectedClients.Values)
            {
                client.sender.handle.Close();
            }
            connectedClients.Clear();
            cursorMap.Clear();
            isClientConnected = false;
            base.Reset();
        }

        #endregion

        #region IWspAdapter Members

        #region CPMConnectIn and CPMConnectOut

        /// <summary>
        /// CPMConnectInRequest() is used to send a request to establish
        /// a connection with the server and starts WSP query processing
        /// or WSP catalog administration.
        /// </summary>
        public void CPMConnectInRequest()
        {
            int remoteClient = 1;
            CPMConnectInRequest(clientVersion, remoteClient, catalogName);
        }

        /// <summary>
        /// Used to send a request to establish a connection with the server
        /// and start WSP query processing or WSP catalog administration
        /// </summary>
        /// <param name="clientVersion">Indicate whether the server is to validate the checksum</param>
        /// <param name="isClientRemote">Indicate if the client is running on a different machine than the server</param>
        /// <param name="catalogName">The name of the catalog</param>
        /// <param name="cPropSets">The number of CDbPropSet structures in the following fields</param>
        /// <param name="cExtPropSet">The number of CDbPropSet structures in aPropertySet</param>
        public void CPMConnectInRequest(uint clientVersion, int isClientRemote, string catalogName, uint cPropSets = 2, uint cExtPropSet = 4)
        {
            WspClient client;

            string locale = wspTestSite.Properties.Get("LanguageLocale");

            string serverName = serverMachineName;
            var connectInMessage = builder.GetConnectInMessage(clientVersion, isClientRemote, userName, clientMachineName, serverName, catalogName, locale, cPropSets, cExtPropSet);

            WspMessageHeader? header = GetWspMessageHeader(connectInMessage.Header._msg, connectInMessage.Header._status, connectInMessage.Header._ulChecksum);

            //Send the connectIn message to Server.
            if (!connectedClients.ContainsKey(clientMachineName))
            {
                client = new WspClient();
                client.sender = new RequestSender(pipePath);
                connectedClients.Add(clientMachineName, client);
            }
            else
            {
                client = connectedClients[clientMachineName];
            }
            // Send CPMConnectIn Message
            //Write the message in the Pipe and Get the response 
            //in the outputBuffer

            var connectInMessageBytes = Helper.ToBytes(connectInMessage);
            uint checkSum = GetCheckSumField(connectInMessageBytes);
            client.SendCPMConnectIn(connectInMessage._iClientVersion, connectInMessage._fClientIsRemote, connectInMessage.MachineName, connectInMessage.UserName, connectInMessage.PropertySet1, connectInMessage.PropertySet2, connectInMessage.aPropertySets, cPropSets: connectInMessage.cPropSets, cExtPropSet: connectInMessage.cExtPropSet, calculateChecksum: calculateChecksum, header: header);
            // RequestSender objects uses path '\\pipe\\MSFTEWDS'
            // for the protocol transport
            wspTestSite.CaptureRequirement(3, @"All messages MUST be " +
                 "transported using a named pipe: \\pipe\\MSFTEWDS");

            // Get the errorCode & invoke event
            if (CPMConnectOutResponse != null)
            {
                CPMConnectOut connectOutMessage;

                client.ExpectMessage(out connectOutMessage);

                uint msgId = (UInt32)connectOutMessage.Header._msg;
                uint msgStatus = connectOutMessage.Header._status;
                if (msgStatus != 0)
                {
                    wspTestSite.CaptureRequirement(620,
                        "Whenever an error occurs during processing of a " +
                        "message sent by a client, the server MUST set the " +
                        "_status field to the error code value.");
                }
                if ((msgId == (uint)MessageType.CPMConnectOut)
                    && (msgStatus == 0x00000000))
                {
                    validator.ValidateConnectOutResponse(connectOutMessage);
                    builder.Is64bit = validator.Is64bit;
                    isClientConnected = true;
                }

                //Updated by v-aliche
                //Delta Testing
                startingIndexconnect = 0;
                validator.ValidateHeader
                    (connectInMessageBytes,
                    MessageType.CPMConnectOut,
                    checkSum, ref startingIndexconnect);
                uint obtainedServerVersion
                 = Helper.GetUInt(connectInMessageBytes, ref startingIndexconnect);
                //if serverVersion equals 0x00000102, ir means Windows server 2008 with OS 32-bit
                //if serverVersion equals 0x00010102, ir means Windows server 2008 with OS 64-bit
                if (obtainedServerVersion == 0x00000102 || obtainedServerVersion == 0x00010102)
                {
                    byte[] dWordIn = new byte[4];
                    byte[] dWordOut = new byte[4];
                    Helper.CopyBytes(connectInMessageBytes, ref startingIndexconnect, dWordIn);
                    Helper.CopyBytes(connectInMessageBytes, ref startingIndexconnect, dWordOut);
                    bool isEqual = dWordIn.ToString() == dWordOut.ToString();

                    Site.CaptureRequirementIfIsTrue(isEqual, 1055, "When the server receives a CPMConnectIn request from a client," +
                        "Otherwise[the server not support reporting versioning information]" +
                        "server MUST copy 4 DWORDs at the offset starting after _serverVersion" +
                        "from the CPMConnectIn message to indicate that versioning is not supported.");
                }

                CPMConnectOutResponse(msgStatus);
            }
        }

        /// <summary>
        /// This event is used to get the response from CPMConnectIn request.
        /// </summary>
        public event CPMConnectOutResponseHandler CPMConnectOutResponse;


        #endregion

        #region CPMCreateQueryIn and CPMCreateQueryOut
        /// <summary>
        /// CPMCreateQueryIn() emulates WSP CPMCreateQueryIn message and 
        /// associates a query with the client if successful
        /// </summary>
        public void CPMCreateQueryIn(bool ENABLEROWSETEVENTS)
        {
            string queryScope = wspTestSite.Properties.Get("QueryPath");
            string queryText = wspTestSite.Properties.Get("QueryText");

            var queryInMessage = builder.GetCPMCreateQueryIn(queryScope, queryText, WspConsts.System_Search_Contents, ENABLEROWSETEVENTS);

            CPMCreateQueryIn(
                queryInMessage.ColumnSet,
                queryInMessage.RestrictionArray,
                queryInMessage.SortSet,
                queryInMessage.CCategorizationSet,
                queryInMessage.RowSetProperties,
                queryInMessage.PidMapper,
                queryInMessage.GroupArray,
                queryInMessage.Lcid);
        }

        /// <summary>
        /// Create and send CPMCreateQueryIn and expect response.
        /// </summary>
        public void CPMCreateQueryIn(
            CColumnSet? columnSet,
            CRestrictionArray? restrictionArray,
            CInGroupSortAggregSets? sortSet,
            CCategorizationSet? categorizationSet,
            CRowsetProperties rowsetProperties,
            CPidMapper pidMapper,
            CColumnGroupArray groupArray,
            uint lcid)
        {
            var client = GetClient(isClientConnected);

            client.SendCPMCreateQueryIn(columnSet, restrictionArray, sortSet, categorizationSet, rowsetProperties, pidMapper, groupArray, lcid);

            CPMCreateQueryOut response;
            client.ExpectMessage<CPMCreateQueryOut>(out response);

            if (response.Header._status == 0)
            {
                uint[] cursor = null;// to be obtained from the server
                validator.ValidateCreateQueryOutResponse(response, out cursor);
                cursorMap.Add(clientMachineName, cursor[0]);
            }

            CPMCreateQueryOutResponse(response.Header._status);
        }

        /// <summary>
        /// This event is used to get the response for CPMCreateQueryIn request.
        /// </summary>
        public event CPMCreateQueryOutResponseHandler
            CPMCreateQueryOutResponse;

        #endregion

        #region CPMGetQueryStatusIn and CPMGetQueryStatusOut

        /// <summary>
        /// CPMGetQueryStatusIn() requests the status of the query.
        /// </summary>
        /// <param name="isCursorValid">Indicates the client requesting
        /// status of the query.</param>
        public void CPMGetQueryStatusIn(bool isCursorValid)
        {
            int MAX_RANGE = 500;
            int MIN_RANGE = 100;
            int startingIndex = 0;
            uint cursorAssociated = 0;
            if (isCursorValid)
            {
                cursorAssociated = GetCursor(clientMachineName);
            }
            else
            {
                // Generating a PseudoRandom Number
                Random r = new Random();
                cursorAssociated
                    = uint.MaxValue - (uint)r.Next(MAX_RANGE, MIN_RANGE);
            }
            byte[] statusInMessage
                = builder.GetCPMQueryStatusIn(cursorAssociated);
            byte[] statusOutMessage;
            RequestSender sender = GetRequestSender(isClientConnected);
            bytesRead
                = sender.SendMessage(statusInMessage, out statusOutMessage);
            if (sender == defaultSender)
            {
                //This means that disconnect message has been sent
                // through the pipe which does not have a connect In
                // sent across it.
                // If the sender.SendMessage() method is successful
                // Requirement 598 is validated
                wspTestSite.CaptureRequirement(598,
                   "In Windows the same pipe connection is used for the " +
                   "following messages, except when the error is returned" +
                   "in a CPMConnectOut message.");

            }
            // RequestSender objects uses path '\\pipe\\MSFTEWDS'
            // for the protocol transport
            wspTestSite.CaptureRequirement(3, @"All messages MUST be " +
                "transported using a named pipe: \\pipe\\MSFTEWDS");

            uint checkSum = 0;
            if (CPMGetQueryStatusOutResponse != null)
            {
                startingIndex = 0;
                uint msgId = Helper.GetUInt(statusOutMessage,
                    ref startingIndex);
                uint msgStatus = Helper.GetUInt(statusOutMessage,
                    ref startingIndex);
                if (msgStatus != 0)
                {
                    wspTestSite.CaptureRequirementIfAreEqual<int>(bytesRead,
                    Constant.SIZE_OF_HEADER, 619,
                    "Whenever an error occurs during processing of a " +
                    "message sent by a client, the server MUST respond " +
                    "with the message header (only) of the message sent " +
                    "by the client, keeping the _msg field intact.");
                    //If 4 byte Non Zero field is read as status
                    // The requirement 620 gets validated
                    wspTestSite.CaptureRequirement(620,
                        "Whenever an error occurs during processing of a " +
                        "message sent by a client, the server MUST set the " +
                        "_status field to the error code value.");
                }
                if ((msgId == (uint)MessageType.CPMGetQueryStatusOut)
                    && (msgStatus == 0x00000000))
                {
                    validator.ValidateGetQueryStatusOut(statusOutMessage,
                        checkSum);
                }
                CPMGetQueryStatusOutResponse(msgStatus);
            }
        }


        /// <summary>
        /// This event is used to get the response from 
        /// CPMGetQueryStatusIn request.
        /// </summary>
        public event CPMGetQueryStatusOutResponseHandler
            CPMGetQueryStatusOutResponse;

        #endregion

        #region CPMGetQueryStatusExIn and CPMGetQueryStatusExOut

        /// <summary>
        /// CPMGetQueryStatusExIn()requests for the status of the query and the
        /// additional information from the server.
        /// </summary>
        /// <param name="isCursorValid">Indicates the client requesting 
        /// status has a valid cursor.</param>
        public void CPMGetQueryStatusExIn(bool isCursorValid)
        {
            uint cursorAssociated = 0;
            if (isCursorValid)
            {
                cursorAssociated = GetCursor(clientMachineName);
            }
            else
            {
                cursorAssociated = 2;
            }
            byte[] statusExInMessage
                = builder.GetCPMQueryStatusExIn(cursorAssociated, 1);
            byte[] statusExOutMessage;
            uint checkSum = 0;
            RequestSender sender
                = GetRequestSender(isClientConnected); //Get the Sender
            bytesRead
                = sender.SendMessage(statusExInMessage,
                out statusExOutMessage);
            if (bytesRead == -1)
            {
                int error = Marshal.GetLastWin32Error();
            }
            if (sender == defaultSender)
            {
                // This means that disconnect message has been sent
                // through the pipe which does not have a connect In
                // sent across it.
                // If the sender.SendMessage() method is successful
                // Requirement 598 is validated
                wspTestSite.CaptureRequirement(598,
                   "In Windows the same pipe connection is used for the " +
                   "following messages, except when the error is returned" +
                   "in a CPMConnectOut message.");

            }
            wspTestSite.CaptureRequirement(3, @"All messages MUST be " +
                            "transported using a named pipe: \\pipe\\MSFTEWDS");
            if (CPMGetQueryStatusExOutResponse != null)
            {
                int startingIndex = 0;
                uint msgId
                    = Helper.GetUInt(statusExOutMessage, ref startingIndex);
                uint msgStatus
                    = Helper.GetUInt(statusExOutMessage, ref startingIndex);
                if (msgStatus != 0)
                {
                    wspTestSite.CaptureRequirementIfAreEqual<int>(bytesRead,
        Constant.SIZE_OF_HEADER, 619,
        "Whenever an error occurs during processing of a " +
        "message sent by a client, the server MUST respond " +
        "with the message header (only) of the message sent " +
        "by the client, keeping the _msg field intact.");
                    //If 4 byte Non Zero field is read as status
                    // The requirement 620 gets validated
                    wspTestSite.CaptureRequirement(620,
                        "Whenever an error occurs during processing of a " +
                        "message sent by a client, the server MUST set the " +
                        "_status field to the error code value.");
                }
                if ((msgId == (uint)MessageType.CPMGetQueryStatusExOut)
                    && (msgStatus == 0x00000000))
                {
                    validator.ValidateGetQueryStatusExOut(statusExOutMessage,
                        checkSum);
                }

                if (wspTestSite.Properties.Get("ServerOSVersion").ToUpper() == "WIN7")
                {
                    if (!isCursorValid && msgStatus != 0x00000000)
                    {
                        //MS-WSP_R683
                        wspTestSite.CaptureRequirementIfAreEqual<uint>((uint)WspErrorCode.E_FAIL, msgStatus, 683,
                            "When the server receives a CPMGetQueryStatusExIn message request from a client, " +
                            "the server MUST report an E_FAIL (0x80004005) error if the cursor handle passed is not in a list of the client's cursor handles.");
                    }
                }

                // Fire Response Event
                CPMGetQueryStatusExOutResponse(msgStatus);
            }
        }
        /// <summary>
        /// This event is used to get the response from 
        /// CPMGetQueryStatusExIn request.
        /// </summary>
        public event CPMGetQueryStatusExOutResponseHandler
            CPMGetQueryStatusExOutResponse;

        #endregion

        #region CPMRatioFinishedIn and CPMRatioFinishedOut

        /// <summary>
        /// CPMRatioFinishedIn() requests for the completion percentage of the 
        /// query.
        /// </summary>
        /// <param name="isCursorValid">Indicates the client requesting 
        /// has a valid cursor</param>
        public void CPMRatioFinishedIn(bool isCursorValid)
        {
            uint cursorAssociated = 0;
            if (isCursorValid)
            {
                cursorAssociated = GetCursor(clientMachineName);
            }
            else
            {
                cursorAssociated = uint.MaxValue - 10;
            }

            //Build ratioFinishedIn request
            byte[] ratioFinishedInrequest
                = builder.GetCPMRatioFinishedIn(cursorAssociated, 1);
            byte[] ratioFinishedInResponse;
            //Send ratioFinishedIn message to server
            RequestSender sender = GetRequestSender(isClientConnected);
            bytesRead
                = sender.SendMessage(ratioFinishedInrequest,
                out ratioFinishedInResponse);
            if (sender == defaultSender)
            {
                // This means that disconnect message has been sent
                // through the pipe which does not have a connect In
                // sent across it.
                // If the sender.SendMessage() method is successful
                // Requirement 598 is validated
                wspTestSite.CaptureRequirement(598,
                   "In Windows the same pipe connection is used for the " +
                   "following messages, except when the error is returned" +
                   "in a CPMConnectOut message.");

            }
            // RequestSender objects uses path '\\pipe\\MSFTEWDS'
            // for the protocol transport
            wspTestSite.CaptureRequirement(3, @"All messages MUST be " +
                "transported using a named pipe: \\pipe\\MSFTEWDS");

            uint checkSum = 0;
            if (CPMRatioFinishedOutResponse != null)
            {
                int startingIndex = 0;
                uint msgId
                   = Helper.GetUInt(ratioFinishedInResponse,
                   ref startingIndex);
                uint msgStatus
                    = Helper.GetUInt(ratioFinishedInResponse,
                    ref startingIndex);
                if (msgStatus != 0)
                {
                    wspTestSite.CaptureRequirementIfAreEqual<int>(bytesRead,
                    Constant.SIZE_OF_HEADER, 619,
                    "Whenever an error occurs during processing of a " +
                    "message sent by a client, the server MUST respond " +
                    "with the message header (only) of the message sent " +
                    "by the client, keeping the _msg field intact.");
                    //If 4 byte Non Zero field is read as status
                    // The requirement 620 gets validated
                    wspTestSite.CaptureRequirement(620,
                        "Whenever an error occurs during processing of a " +
                        "message sent by a client, the server MUST set the " +
                        "_status field to the error code value.");
                }
                if ((msgId == (uint)MessageType.CPMRatioFinishedOut)
                    && (msgStatus == 0x00000000))
                {
                    validator.ValidateRatioFinishedOut(ratioFinishedInResponse,
                        checkSum, msgStatus);
                }

                if (wspTestSite.Properties.Get("ServerOSVersion").ToUpper() == "WIN7")
                {
                    if (!isCursorValid && msgStatus != 0x00000000)
                    {
                        //MS-WSP_R696
                        wspTestSite.CaptureRequirementIfAreEqual<uint>((uint)WspErrorCode.E_FAIL, msgStatus, 696,
                            "When the server receives a CPMRatioFinishedIn message request from a client, " +
                            "the server MUST report an E_FAIL (0x80004005) error if the cursor handle passed is not in the list of the clients cursor handles.");
                    }
                }

                CPMRatioFinishedOutResponse(msgStatus); // Fire response event
            }
        }
        /// <summary>
        /// This event is used to get the response from 
        /// CPMRatioFinishedIn request.
        /// </summary>
        public event CPMRatioFinishedOutResponseHandler
            CPMRatioFinishedOutResponse;

        #endregion

        #region CPMSetBindingsIn and CPMSetBindingsInResponse

        /// <summary>
        /// CPMSetBindingsIn() requests the bindings of columns to a rowset.
        /// </summary>
        /// <param name="isValidBinding">Indicates the client requesting 
        /// bindings of columns to a rowset.</param>
        /// <param name="isCursorValid">Indicates whether the cursor used
        /// is valid</param>
        public void CPMSetBindingsIn(bool isValidBinding, bool isCursorValid)
        {
            uint cursorAssociated = isCursorValid ? GetCursor(clientMachineName) : 0;

            var setBindingsInMessage = builder.GetCPMSetBindingsIn(cursorAssociated, out tableColumns, isValidBinding);
            this.setBindingsIn = setBindingsInMessage;

            CPMSetBindingsIn(
                setBindingsInMessage._hCursor,
                setBindingsInMessage._cbRow,
                setBindingsInMessage.cColumns,
                setBindingsInMessage.aColumns);
        }

        /// <summary>
        /// CPMSetBindingsIn() requests the bindings of columns to a rowset.
        /// </summary>
        public void CPMSetBindingsIn(CTableColumn[] aColumns)
        {
            CPMSetBindingsIn(
                GetCursor(clientMachineName),
                MessageBuilder.rowWidth,
                (uint)aColumns.Length,
                aColumns);
        }

        /// <summary>
        /// Create and send CPMSetBindingsIn and expect response.
        /// </summary>
        public void CPMSetBindingsIn(uint _hCursor, uint _cbRow, uint cColumns, CTableColumn[] aColumns)
        {
            var client = GetClient(isClientConnected);

            Helper.UpdateTableColumns(aColumns);
            client.SendCPMSetBindingsIn(_hCursor, _cbRow, cColumns, aColumns);

            CPMSetBindingsOut response;
            client.ExpectMessage<CPMSetBindingsOut>(out response);

            if (client == defaultClient)
            {
                // This means that disconnect message has been sent
                // through the pipe which does not have a connect In
                // sent across it.
                // If the sender.SendMessage() method is successful
                // Requirement 598 is validated
                wspTestSite.CaptureRequirement(598,
                   "In Windows the same pipe connection is used for the " +
                   "following messages, except when the error is returned" +
                   "in a CPMConnectOut message.");

            }
            // RequestSender objects uses path '\\pipe\\MSFTEWDS'
            // for the protocol transport
            wspTestSite.CaptureRequirement(3, @"All messages MUST be " +
                         "transported using a named pipe: \\pipe\\MSFTEWDS");

            uint msgId = (UInt32)response.Header._msg;
            uint msgStatus = response.Header._status;
            if (msgStatus != 0)
            {
                //If 4 byte Non Zero field is read as status
                // The requirement 620 gets validated
                wspTestSite.CaptureRequirement(620,
                    "Whenever an error occurs during processing of a " +
                    "message sent by a client, the server MUST set the " +
                    "_status field to the error code value.");

            }
            if ((msgId == (uint)MessageType.CPMSetBindingsIn)
                && (msgStatus == 0x00000000))
            {
                validator.ValidateSetBindingsInResponse(response);
            }
            CPMSetBindingsInResponse(msgStatus); // Fire Response Event
        }

        /// <summary>
        /// This event is used to get the response from 
        /// CPMSetBindingsIn request.
        /// </summary>
        public event CPMSetBindingsInResponseHandler
            CPMSetBindingsInResponse;

        #endregion

        #region CPMGetRowsIn and CPMGetRowsOut

        /// <summary>
        /// CPMGetRowsIn() message requests rows from a query.
        /// </summary>
        /// <param name="isCursorValid">Indicates the client 
        /// requesting row has a valid cursor.</param>
        public void CPMGetRowsIn(bool isCursorValid)
        {
            uint cursorAssociated;
            if (isCursorValid)
            {
                cursorAssociated = GetCursor(clientMachineName);
            }
            else
            {
                Random r = new Random();
                cursorAssociated = (uint)r.Next(50, 60);
            }

            CPMGetRowsIn(cursorAssociated, builder.parameter.RowsToTransfer, builder.parameter.EachRowSize, builder.parameter.BufferSize, 0, builder.parameter.EType, out CPMGetRowsOut getRowsOut);
        }

        /// <summary>
        /// CPMGetRowsIn() message requests rows from a query.
        /// </summary>
        public void CPMGetRowsIn(out CPMGetRowsOut getRowsOut)
        {
            uint cursorAssociated = GetCursor(clientMachineName);
            getRowsOut = null;
            CPMGetRowsIn(cursorAssociated, builder.parameter.RowsToTransfer, builder.parameter.EachRowSize, builder.parameter.BufferSize, 0, builder.parameter.EType, out getRowsOut);
        }

        /// <summary>
        /// CPMGetRowsIn() message requests rows from a query.
        /// </summary>
        /// <param name="cursor">Representing the handle from the CPMCreateQueryOut message identifying the query for which to retrieve rows. </param>
        /// <param name="rowsToTransfer">Indicating the maximum number of rows that the client will receive in response to this message</param>
        /// <param name="rowWidth">Indicating the length of a row, in bytes</param>
        /// <param name="cbReadBuffer">This field MUST be set to the maximum of the value of _cbRowWidth or 1000 times the value of _cRowsToTransfer, rounded up to the nearest 512 byte multiple. The value MUST NOT exceed 0x00004000</param>
        /// <param name="fBwdFetch">Indicating the order in which to fetch the rows</param>
        /// <param name="eType">Type of SeekDescription</param>
        public void CPMGetRowsIn(uint cursor, uint rowsToTransfer, uint rowWidth, uint cbReadBuffer, uint fBwdFetch, uint eType, out CPMGetRowsOut response)
        {
            // Get hold of appropriate Sender (Pipe with/without connection)
            var client = GetClient(isClientConnected);
            var getRowsInMessage = builder.GetCPMRowsInMessage(cursor, rowsToTransfer, rowWidth, cbReadBuffer, fBwdFetch, eType, out rowsInReserve);
            var getRowsInMessageBytes = Helper.ToBytes(getRowsInMessage);

            client.SendCPMGetRowsIn(getRowsInMessage._hCursor, getRowsInMessage._cRowsToTransfer, getRowsInMessage._cbRowWidth, getRowsInMessage._cbSeek, getRowsInMessage._cbReserved, getRowsInMessage._cbReadBuffer, getRowsInMessage._ulClientBase, getRowsInMessage._fBwdFetch, getRowsInMessage.eType, getRowsInMessage._chapt, getRowsInMessage.SeekDescription);

            if (client == defaultClient)
            {
                // This means that disconnect message has been sent
                // through the pipe which does not have a connect In
                // sent across it.
                // If the sender.SendMessage() method is successful
                // Requirement 598 is validated
                wspTestSite.CaptureRequirement(598,
                   "In Windows the same pipe connection is used for the " +
                   "following messages, except when the error is returned" +
                   "in a CPMConnectOut message.");
            }
            // RequestSender objects uses path '\\pipe\\MSFTEWDS'
            // for the protocol transport
            wspTestSite.CaptureRequirement(3, @"All messages MUST be " +
                "transported using a named pipe: \\pipe\\MSFTEWDS");

            response = new CPMGetRowsOut();
            if (CPMGetRowsOut != null)
            {
                client.ExpectMessage<CPMGetRowsOut>(out response);

                uint offsetUsed = GetOffsetUsed();
                validator.ValidateGetRowsOut(response, out lastDocumentWorkId);

                // Fire Response Event
                CPMGetRowsOut(response.Header._status);
            }
        }

        /// <summary>
        /// This event is used to get the response from CPMGetRowsIn request.
        /// </summary>
        public event CPMGetRowsOutResponseHandler CPMGetRowsOut;

        #endregion

        #region CPMFetchValueIn and CPMFetchValueOut

        /// <summary>
        /// CPMFetchValueIn() requests for the property value that 
        /// was too large
        /// to return in a rowset.
        /// </summary>
        /// <param name="isCursorValid">Indicates whether the client 
        /// has a valid cursor associated</param>
        public void CPMFetchValueIn(bool isCursorValid)
        {
            uint cbSoFar = 0;
            uint cbChunk
                = Convert.ToUInt32(wspTestSite.Properties["CbChunk"]);
            uint cursor = GetCursor(clientMachineName);
            uint workId = GetWorkIdFromRowsOutMessage();
            byte[] fetchValueOut = null;
            byte[] value = null;
            byte[] fetchValueIn
                = builder.GetCPMFetchValueIn(workId, cbSoFar, cbChunk);
            uint checkSum = GetCheckSumField(fetchValueIn);
            RequestSender sender = GetRequestSender(isClientConnected);
            bytesRead = sender.SendMessage(fetchValueIn, out fetchValueOut);
            value = new byte[bytesRead];
            Array.Copy(fetchValueOut, 0, value, 0, bytesRead);
            int index = 4;
            // Read the message Response 
            uint msgStatus = Helper.GetUInt(fetchValueOut, ref index);
            if (msgStatus == 0)
            {
                validator.ValidateFetchValueOut(value,
                    checkSum, cbSoFar, cbChunk);
            }
            CPMFetchValueOutResponse(msgStatus);
        }

        private uint GetWorkIdFromRowsOutMessage()
        {
            return lastDocumentWorkId;
        }
        /// <summary>
        /// this event is used to get the response from 
        /// cpmfetchvaluein request.
        /// </summary>
        public event CPMFetchValueOutResponseHandler
            CPMFetchValueOutResponse;

        #endregion

        #region CPMCiStateInOut and CPMCiStateInOutResponse

        /// <summary>
        /// CPMCiStateInOut() requests the information about the 
        /// state of the Windows Search Service.
        /// </summary>
        public void CPMCiStateInOut()
        {
            var ciStateInOutMessage = builder.GetCPMCiStateInOut();
            var ciStateInOutMessageBytes = Helper.ToBytes(ciStateInOutMessage);
            byte[] ciStateInOutResponseMessageBytes;
            RequestSender sender
                = GetRequestSender(isClientConnected); //Get the Sender
            bytesRead = sender.SendMessage(ciStateInOutMessageBytes, out ciStateInOutResponseMessageBytes);
            if (sender == defaultSender)
            {
                // This means that disconnect message has been sent
                // through the pipe which does not have a connect In
                // sent across it.
                // If the sender.SendMessage() method is successful
                // Requirement 598 is validated
                wspTestSite.CaptureRequirement(598,
                   "In Windows the same pipe connection is used for the " +
                   "following messages, except when the error is returned" +
                   "in a CPMConnectOut message.");

            }
            // RequestSender objects uses path '\\pipe\\MSFTEWDS'
            // for the protocol transport
            wspTestSite.CaptureRequirement(3, @"All messages MUST be " +
                  "transported using a named pipe: \\pipe\\MSFTEWDS");

            if (ciStateInOutResponseMessageBytes != null)
            {
                var header = new WspMessageHeader();
                Helper.FromBytes(ref header, ciStateInOutResponseMessageBytes);

                uint msgId = (UInt32)header._msg;
                uint msgStatus = header._status;
                if (msgStatus != 0)
                {
                    wspTestSite.CaptureRequirementIfAreEqual<int>(bytesRead,
                        Constant.SIZE_OF_HEADER, 619,
                        "Whenever an error occurs during processing of a " +
                        "message sent by a client, the server MUST respond " +
                        "with the message header (only) of the message sent " +
                        "by the client, keeping the _msg field intact.");
                    //If 4 byte Non Zero field is read as status
                    // The requirement 620 gets validated
                    wspTestSite.CaptureRequirement(620,
                        "Whenever an error occurs during processing of a " +
                        "message sent by a client, the server MUST set the " +
                        "_status field to the error code value.");
                }
                if (msgStatus == 0)
                {
                    var response = new CPMCiStateInOut();
                    Helper.FromBytes(ref response, ciStateInOutResponseMessageBytes);

                    validator.ValidateCiStateInOut(response);
                }
                CPMCiStateInOutResponse(msgStatus); // Fire Response Event
            }
        }
        /// <summary>
        /// This event is used to get the response from 
        /// CPMCiStateInOut request.
        /// </summary>
        public event CPMCiStateInOutResponseHandler
            CPMCiStateInOutResponse;

        #endregion

        #region CPMGetNotifyIn and CPMSendNotifyOut

        /// <summary>
        /// CPMGetNotifyIn() requests that the client wants to be notified
        /// of rowset changes.
        /// </summary>
        /// <param name="isCursorValid">Indicates whether the 
        /// client has valid query cursor</param>
        public void CPMGetNotify(bool isCursorValid)
        {
            uint cursorAssociated = 0;
            if (isCursorValid)
            {
                cursorAssociated = GetCursor(clientMachineName);
            }
            else
            {
                cursorAssociated = uint.MaxValue - 10;
            }

            byte[] getNotifyMessage = builder.GetCPMGetNotify();
            byte[] sendNotifyOut;
            uint checkSum = 0;
            RequestSender sender
                = GetRequestSender(isClientConnected); //Get the Sender
            sender.SendMessage(getNotifyMessage, out sendNotifyOut);
            if (sender == defaultSender)
            {
                // This means that disconnect message has been sent
                // through the pipe which does not have a connect In
                // sent across it.
                // If the sender.SendMessage() method is successful
                // Requirement 598 is validated
                wspTestSite.CaptureRequirement(598,
                   "In Windows the same pipe connection is used for the " +
                   "following messages, except when the error is returned" +
                   "in a CPMConnectOut message.");

            }
            // RequestSender objects uses path '\\pipe\\MSFTEWDS'
            // for the protocol transport
            wspTestSite.CaptureRequirement(3, @"All messages MUST be " +
                "transported using a named pipe: \\pipe\\MSFTEWDS");

            if (CPMSendNotifyOutResponse != null)
            {
                int startingIndex = 0;
                uint msgId
                    = Helper.GetUInt(sendNotifyOut, ref startingIndex);
                uint msgStatus
                    = Helper.GetUInt(sendNotifyOut, ref startingIndex);
                if (msgStatus != 0)
                {
                    if (wspTestSite.Properties.Get("ServerOSVersion").ToUpper() != "WIN7")
                    {
                        wspTestSite.CaptureRequirementIfAreEqual<int>(bytesRead,
                        Constant.SIZE_OF_HEADER, 619,
                        "Whenever an error occurs during processing of a " +
                        "message sent by a client, the server MUST respond " +
                        "with the message header (only) of the message sent " +
                        "by the client, keeping the _msg field intact.");
                    }

                    //If 4 byte Non Zero field is read as status
                    // The requirement 620 gets validated
                    wspTestSite.CaptureRequirement(620,
                        "Whenever an error occurs during processing of a " +
                        "message sent by a client, the server MUST set the " +
                        "_status field to the error code value.");
                }
                if ((msgId == (uint)MessageType.CPMSendNotifyOut)
                    && (msgStatus == 0x00000000))
                {
                    validator.ValidateSendNotifyOut(sendNotifyOut, checkSum);
                }
                CPMSendNotifyOutResponse(msgStatus); // Fire Response Event
            }
        }
        /// <summary>
        /// This event is used to get the response from CPMGetNotify request.
        /// </summary>
        public event CPMSendNotifyOutResponseHandler CPMSendNotifyOutResponse;

        #endregion

        #region CPMFreeCursorIn and CPMFreeCursorOut

        /// <summary>
        /// CPMFreeCursorIn() requests to release a cursor.
        /// </summary>
        /// <param name="isCursorValid">Indicates the client requesting 
        /// to release a cursor.</param>
        public void CPMFreeCursorIn(bool isCursorValid)
        {
            uint cursorAssociated = 0;
            if (isCursorValid)
            {
                cursorAssociated = GetCursor(clientMachineName);
            }
            else
            {
                cursorAssociated = uint.MaxValue - 10;
            }
            uint errorCode = 0xFFFFFFFF; // Get it from Server
                                         //string machineName = GetMachineName(machineNameValue);
                                         //uint cursor = GetCursor(clientMachineName);
            byte[] freeCursorOut;
            byte[] freeCursor = builder.GetFreeCursorIn(cursorAssociated);
            RequestSender sender = GetRequestSender(isClientConnected);
            sender.SendMessage(freeCursor, out freeCursorOut);
            if (sender == defaultSender)
            {
                // This means that disconnect message has been sent
                // through the pipe which does not have a connect In
                // sent across it.
                // If the sender.SendMessage() method is successful
                // Requirement 598 is validated
                wspTestSite.CaptureRequirement(598,
                   "In Windows the same pipe connection is used for the " +
                   "following messages, except when the error is returned" +
                   "in a CPMConnectOut message.");

            }
            // RequestSender objects uses path '\\pipe\\MSFTEWDS'
            // for the protocol transport
            wspTestSite.CaptureRequirement(3, @"All messages MUST be " +
                "transported using a named pipe: \\pipe\\MSFTEWDS");
            int startingIndex = 0;
            uint msgId
                    = Helper.GetUInt(freeCursorOut, ref startingIndex);
            errorCode = Helper.GetUInt(freeCursorOut, ref startingIndex);
            if (CPMFreeCursorOutResponse != null)
            {
                uint checkSumZero = 0x00000000;
                if ((msgId == (uint)MessageType.CPMFreeCursorOut))
                {
                    validator.ValidateFreeCursorOut(freeCursorOut, checkSumZero);
                }
                CPMFreeCursorOutResponse(errorCode);
            }
        }
        /// <summary>
        /// This event is used to get the response from 
        /// CPMFreeCursorIn request.
        /// </summary>
        public event CPMFreeCursorOutResponseHandler
            CPMFreeCursorOutResponse;

        #endregion

        #region CPMDisconnect

        /// <summary>
        /// CPMDisconnect() request is sent to end the connection 
        /// with the server.
        /// </summary>
        public void CPMDisconnect()
        {
            byte[] disconnectResponse = null;
            RequestSender sender = null;
            byte[] disconnectMessage = builder.GetDisconnectMessage();
            if (connectedClients.ContainsKey(clientMachineName))
            {
                sender = connectedClients[clientMachineName].sender;
            }
            else
            {
                sender = defaultSender;
            }
            int bytesRead = sender.SendMessage(disconnectMessage,
                out disconnectResponse);
            if (sender == defaultSender)
            {
                // This means that disconnect message has been sent
                // through the pipe which does not have a connect In
                // sent across it.
                // If the sender.SendMessage() method is successful
                // Requirement 598 is validated
                wspTestSite.CaptureRequirement(598,
                   "In Windows the same pipe connection is used for the " +
                   "following messages, except when the error is returned" +
                   "in a CPMConnectOut message.");

            }
            // RequestSender objects uses path '\\pipe\\MSFTEWDS'
            // for the protocol transport
            wspTestSite.CaptureRequirement(3, @"All messages MUST be " +
                "transported using a named pipe: \\pipe\\MSFTEWDS");

            if (connectedClients.ContainsKey(clientMachineName))
            {
                // Remove the Request Sender associated with the client
                connectedClients.Remove(clientMachineName);
            }
        }

        #endregion

        #region CPMFindIndicesIn and CPMFindIndicesOut

        /// <summary>
        /// CPMFindIndicesIn() request the rowset position of the next occurrence of a document identifier
        /// </summary>
        /// <param name="isCursorValid"></param>
        public void CPMFindIndicesIn(bool isCursorValid)
        {
            uint cWids = 1;
            uint cDepthPrev = 0;
            byte[] findIndicesOut = null;
            byte[] value = null;
            byte[] findIndicesIn
                = builder.GetCPMFindIndices(cWids, cDepthPrev);
            uint checkSum = GetCheckSumField(findIndicesIn);
            RequestSender sender = GetRequestSender(isClientConnected);
            bytesRead = sender.SendMessage(findIndicesIn, out findIndicesOut);
            value = new byte[bytesRead];
            Array.Copy(findIndicesOut, 0, value, 0, bytesRead);
            int startingIndex = 0;
            // Read the message Response
            uint msgId = Helper.GetUInt(findIndicesOut, ref startingIndex);
            uint errorCode = Helper.GetUInt(findIndicesOut, ref startingIndex);

            //If HRESULT is DB_S_ENDOFROWSET and the thirty-first bit is not set, it should be success
            if (errorCode != 0)
            {
                startingIndex = 4;
                if (VerifyHResult(findIndicesOut, ref startingIndex))
                {
                    errorCode = 0;
                    //if return code is DB_S_ENDOFROWSET, it is not equal 0, but it still shows successfully.
                    Site.CaptureRequirement(1201,
                        "[Windows Search Protocol (WSP) messages indicate success two ways,second way]An HRESULT success value, " +
                        "[such as DB_S_ENDOFROWSET]in whichAn HRESULT [An HRESULT ] the thirty-first bit is not set.");
                }
            }

            if (errorCode == 0)
            {
                validator.ValidateFindIndicesOut(value, checkSum, cDepthPrev);
            }

            CPMFindIndicesOutResponse(errorCode);
        }

        /// <summary>
        /// This event is used to get the response from 
        /// CPMFindIndicesIn request.
        /// </summary>
        public event CPMFindIndicesOutResponseHandler
            CPMFindIndicesOutResponse;
        #endregion

        #region CPMGetRowsetNotidyIn and CPMGetRowsetNotidyOut
        /// <summary>
        /// CPMGetRowsetNotifyIn message requests the next rowset event from the server if available
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="additionalRowsetEvent"></param>
        public void CPMGetRowsetNotifyIn(int eventType, bool additionalRowsetEvent)
        {
            IWSPSUTAdapter sutAdapter = Site.GetAdapter<IWSPSUTAdapter>();
            string fileName;
            int sutStatus;
            switch (eventType)
            {
                case (int)EventType.PROPAGATE_ADD:
                    fileName = wspTestSite.Properties["NewFile1"];
                    //Create a file on remote server
                    sutStatus = sutAdapter.CreateFile(wspTestSite.Properties["ServerComputerName"], wspTestSite.Properties["DomainName"], wspTestSite.Properties["UserName"], wspTestSite.Properties["Password"], fileName);
                    if (sutStatus != 0)
                    {
                        wspTestSite.Log.Add(LogEntryKind.Comment, "File created failed on server");
                        return;
                    }
                    if (additionalRowsetEvent)
                    {
                        fileName = wspTestSite.Properties["NewFile2"];
                        //Create a file on remote server
                        sutStatus = sutAdapter.CreateFile(wspTestSite.Properties["ServerComputerName"], wspTestSite.Properties["DomainName"], wspTestSite.Properties["UserName"], wspTestSite.Properties["Password"], fileName);

                        if (sutStatus != 0)
                        {
                            wspTestSite.Log.Add(LogEntryKind.Comment, "File created failed on server");
                            return;
                        }
                    }
                    break;
                case (int)EventType.PROPAGATE_DELETE:
                    fileName = wspTestSite.Properties["ExistFile1"];
                    //Delete a file on remote server
                    sutStatus = sutAdapter.DeleteFile(wspTestSite.Properties["ServerComputerName"], wspTestSite.Properties["DomainName"], wspTestSite.Properties["UserName"], wspTestSite.Properties["Password"], fileName);
                    if (sutStatus != 0)
                    {
                        wspTestSite.Log.Add(LogEntryKind.Comment, "File deleted failed on server");
                        return;
                    }
                    if (additionalRowsetEvent)
                    {
                        fileName = wspTestSite.Properties["ExistFile2"];
                        //Delete a file on remote server
                        sutStatus = sutAdapter.DeleteFile(wspTestSite.Properties["ServerComputerName"], wspTestSite.Properties["DomainName"], wspTestSite.Properties["UserName"], wspTestSite.Properties["Password"], fileName);
                        if (sutStatus != 0)
                        {
                            wspTestSite.Log.Add(LogEntryKind.Comment, "File deleted failed on server");
                            return;
                        }
                    }
                    break;
                case (int)EventType.PROPAGATE_MODIFY:
                    fileName = wspTestSite.Properties["ExistFile3"];
                    //Modify a file on remote server
                    sutStatus = sutAdapter.ModifyFile(wspTestSite.Properties["ServerComputerName"], wspTestSite.Properties["DomainName"], wspTestSite.Properties["UserName"], wspTestSite.Properties["Password"], fileName);
                    if (sutStatus != 0)
                    {
                        wspTestSite.Log.Add(LogEntryKind.Comment, "File operation failed on server while modifying");
                        return;
                    }
                    if (additionalRowsetEvent)
                    {
                        fileName = wspTestSite.Properties["ExistFile4"];
                        //Modify a file on remote server
                        sutStatus = sutAdapter.ModifyFile(wspTestSite.Properties["ServerComputerName"], wspTestSite.Properties["DomainName"], wspTestSite.Properties["UserName"], wspTestSite.Properties["Password"], fileName);
                        if (sutStatus != 0)
                        {
                            wspTestSite.Log.Add(LogEntryKind.Comment, "File operation failed on server while modifying");
                            return;
                        }
                    }
                    break;
                case (int)EventType.PROPAGATE_NONE:
                default:
                    break;
            }

            byte[] getRowsetNotifyOutMessage = null;
            RequestSender sender = GetRequestSender(isClientConnected);
            byte[] getRowsetNotifyInMessage = builder.GetCPMGetRowsetNotify();
            int bytesRead = sender.SendMessage(getRowsetNotifyInMessage,
                out getRowsetNotifyOutMessage);
            byte[] value = null;
            value = new byte[bytesRead];
            Array.Copy(getRowsetNotifyOutMessage, 0, value, 0, bytesRead);

            uint checkSum = GetCheckSumField(getRowsetNotifyInMessage);

            int startingIndex = 0;
            uint msgId = Helper.GetUInt(getRowsetNotifyOutMessage, ref startingIndex);
            uint errorCode = Helper.GetUInt(getRowsetNotifyOutMessage, ref startingIndex);

            if (errorCode == 0)
            {
                validator.ValidateGetRowsetNotifyOut(value, checkSum, eventType, additionalRowsetEvent);
            }
            CPMGetRowsetNotifyOutResponse(errorCode);

            //Clean the environment for the next CPMGetRowsetNotifyIn call
            CleanUpSettings(eventType, additionalRowsetEvent);
        }

        /// <summary>
        /// Reseting the files for CPMGetRowsetNotifyIn method
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="additionalRowsetEvent"></param>
        public void CleanUpSettings(int eventType, bool additionalRowsetEvent)
        {
            string fileName;
            IWSPSUTAdapter sutAdapter = Site.GetAdapter<IWSPSUTAdapter>();
            int sutStatus;
            switch (eventType)
            {
                case (int)EventType.PROPAGATE_ADD:
                    if (additionalRowsetEvent)
                    {
                        fileName = wspTestSite.Properties["NewFile2"];
                        //Delete file on the remote server
                        sutStatus = sutAdapter.DeleteFile(wspTestSite.Properties["ServerComputerName"], wspTestSite.Properties["DomainName"], wspTestSite.Properties["UserName"], wspTestSite.Properties["Password"], fileName);

                        if (sutStatus != 0)
                        {
                            wspTestSite.Log.Add(LogEntryKind.Comment, "File deleted failed on server");
                            return;
                        }
                    }
                    fileName = wspTestSite.Properties["NewFile1"];
                    //Delete file on the remote server
                    sutStatus = sutAdapter.DeleteFile(wspTestSite.Properties["ServerComputerName"], wspTestSite.Properties["DomainName"], wspTestSite.Properties["UserName"], wspTestSite.Properties["Password"], fileName);

                    if (sutStatus != 0)
                    {
                        wspTestSite.Log.Add(LogEntryKind.Comment, "File deleted failed on server");
                        return;
                    }
                    break;
                case (int)EventType.PROPAGATE_DELETE:
                    fileName = wspTestSite.Properties["ExistFile1"];
                    //Create file on the remote server
                    sutStatus = sutAdapter.CreateFile(wspTestSite.Properties["ServerComputerName"], wspTestSite.Properties["DomainName"], wspTestSite.Properties["UserName"], wspTestSite.Properties["Password"], fileName);
                    if (sutStatus != 0)
                    {
                        wspTestSite.Log.Add(LogEntryKind.Comment, "File created failed on server");
                        return;
                    }
                    if (additionalRowsetEvent)
                    {
                        fileName = wspTestSite.Properties["ExistFile2"];
                        //Create file on the remote server
                        sutStatus = sutAdapter.CreateFile(wspTestSite.Properties["ServerComputerName"], wspTestSite.Properties["DomainName"], wspTestSite.Properties["UserName"], wspTestSite.Properties["Password"], fileName);
                        if (sutStatus != 0)
                        {
                            wspTestSite.Log.Add(LogEntryKind.Comment, "File created failed on server");
                            return;
                        }
                    }
                    break;
                case (int)EventType.PROPAGATE_MODIFY:
                    if (additionalRowsetEvent)
                    {
                        fileName = wspTestSite.Properties["ExistFile4"];
                        //Delete file on the remote server
                        sutStatus = sutAdapter.DeleteFile(wspTestSite.Properties["ServerComputerName"], wspTestSite.Properties["DomainName"], wspTestSite.Properties["UserName"], wspTestSite.Properties["Password"], fileName);
                        if (sutStatus != 0)
                        {
                            wspTestSite.Log.Add(LogEntryKind.Comment, "File deleted failed on server");
                            return;
                        }
                    }
                    fileName = wspTestSite.Properties["ExistFile3"];
                    //Delete file on the remote server
                    sutStatus = sutAdapter.DeleteFile(wspTestSite.Properties["ServerComputerName"], wspTestSite.Properties["DomainName"], wspTestSite.Properties["UserName"], wspTestSite.Properties["Password"], fileName);
                    if (sutStatus != 0)
                    {
                        wspTestSite.Log.Add(LogEntryKind.Comment, "File deleted failed on server");
                        return;
                    }
                    //Create file on the remote server
                    sutStatus = sutAdapter.CreateFile(wspTestSite.Properties["ServerComputerName"], wspTestSite.Properties["DomainName"], wspTestSite.Properties["UserName"], wspTestSite.Properties["Password"], fileName);
                    if (sutStatus != 0)
                    {
                        wspTestSite.Log.Add(LogEntryKind.Comment, "File created failed on server");
                        return;
                    }
                    if (additionalRowsetEvent)
                    {
                        fileName = wspTestSite.Properties["ExistFile4"];
                        //Create file on the remote server
                        sutStatus = sutAdapter.CreateFile(wspTestSite.Properties["ServerComputerName"], wspTestSite.Properties["DomainName"], wspTestSite.Properties["UserName"], wspTestSite.Properties["Password"], fileName);
                        if (sutStatus != 0)
                        {
                            wspTestSite.Log.Add(LogEntryKind.Comment, "File created failed on server");
                            return;
                        }
                    }
                    break;
                case (int)EventType.PROPAGATE_NONE:
                default:
                    break;
            }

        }

        /// <summary>
        /// Verify the error code if it is the HResult success value
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="startingIndex"></param>
        /// <returns></returns>
        public bool VerifyHResult(byte[] bytes, ref int startingIndex)
        {
            bool success = false;
            byte[] temp = new byte[Constant.SIZE_OF_UINT];
            temp = Helper.GetData(bytes, ref startingIndex, Constant.SIZE_OF_UINT);
            if (Convert.ToInt32(temp[3]) == 0)
            {
                //if the last bit is not set, it should be shown as success
                success = true;
            }

            return success;
        }

        /// <summary>
        /// This event is used to get the response from 
        /// CPMGetRowsetNotifyIn request.
        /// </summary>
        public event CPMGetRowsetNotifyOutResponseHandler
            CPMGetRowsetNotifyOutResponse;
        #endregion

        #region CPMGetScopeStatisticsIn and CMPGetScopeStatisticsOut

        /// <summary>
        /// CPMGetScopeStatisticsIn() request that statistics regarding the number of indexed items
        /// </summary>
        public void CPMGetScopeStatisticsIn()
        {
            byte[] statusInMessage
                = builder.GetCPMGetScopeStatisticsIn();
            byte[] statusOutMessage;
            RequestSender sender = GetRequestSender(isClientConnected);
            bytesRead
                = sender.SendMessage(statusInMessage, out statusOutMessage);
            if (sender == defaultSender)
            {
                //This means that disconnect message has been sent
                // through the pipe which does not have a connect In
                // sent across it.
                // If the sender.SendMessage() method is successful
                // Requirement 598 is validated
                wspTestSite.CaptureRequirement(598,
                   "In Windows the same pipe connection is used for the " +
                   "following messages, except when the error is returned" +
                   "in a CPMConnectOut message.");

            }
            // RequestSender objects uses path '\\pipe\\MSFTEWDS'
            // for the protocol transport
            wspTestSite.CaptureRequirement(3, @"All messages MUST be " +
                "transported using a named pipe: \\pipe\\MSFTEWDS");

            uint checkSum = 0;
            if (CPMGetQueryStatusOutResponse != null)
            {
                int startingIndex = 0;
                uint msgId = Helper.GetUInt(statusOutMessage,
                    ref startingIndex);
                uint msgStatus = Helper.GetUInt(statusOutMessage,
                    ref startingIndex);


                if ((msgId == (uint)MessageType.CPMGetScopeStatisticsOut)
                    && (msgStatus == 0x00000000))
                {
                    validator.ValidateGetScopeStatisticsOut(statusOutMessage, checkSum, msgStatus);

                    //if message response successfully, it must populated with the appropriate statistics
                    wspTestSite.CaptureRequirement(1153,
                        "[When the server receives a CPMGetScopeStatisticsIn message request from a client, " +
                        "the server MUST]The CPMGetScopeStatisticsOut message MUST be populated with the appropriate statistics.");
                }

                CPMGetScopeStatisticsOutResponse(msgStatus);
            }
        }
        /// <summary>
        /// This is to get the response for CMPGetScopeStatistics
        /// </summary>
        public event CPMGetScopeStatisticsOutResponseHandler CPMGetScopeStatisticsOutResponse;

        #endregion

        #region CPMSetScopePrioritizationIn and CPMSetScopePrioritizationOut

        /// <summary>
        /// CPMSetScopePrioritizationIn() request that server prioritize indexing of items that may be relevant to the originating query
        /// at a rate specified in the message
        /// </summary>
        /// <param name="priority"></param>
        public void CPMSetScopePrioritizationIn(uint priority)
        {
            byte[] setScopePrioritizationOutMessage = null;
            RequestSender sender = GetRequestSender(isClientConnected);
            uint eventFrequency = 0;
            if (priority == (uint)Priority.PRIORITY_LEVEL_DEFAULT)
            {
                eventFrequency = 0;
            }
            else
            {
                eventFrequency = 0x00001000;
            }
            byte[] setScopePrioritizationInMessage = builder.GetCPMSetScopePrioritization(priority, eventFrequency);

            int bytesRead = sender.SendMessage(setScopePrioritizationInMessage,
                out setScopePrioritizationOutMessage);
            byte[] value = null;
            value = new byte[bytesRead];
            Array.Copy(setScopePrioritizationOutMessage, 0, value, 0, bytesRead);

            uint checkSum = GetCheckSumField(setScopePrioritizationInMessage);
            // Read the message Response           

            int startingIndex = 0;
            uint msgId = Helper.GetUInt(setScopePrioritizationOutMessage, ref startingIndex);
            uint errorCode = Helper.GetUInt(setScopePrioritizationOutMessage, ref startingIndex);

            if (errorCode == 0)
            {
                validator.ValidateSetScopePrioritizationOut(value, checkSum, eventFrequency);
            }
            CPMSetScopePrioritizationOutResponse(errorCode);
        }

        /// <summary>
        /// This event is used to get the response from 
        /// CPMSetScopePrioritizationIn request.
        /// </summary>
        public event CPMSetScopePrioritizationOutResponseHandler
            CPMSetScopePrioritizationOutResponse;
        #endregion

        #region CPMUpdateDocumentsIn and CPMUpdateDocumentsOut

        /// <summary>
        /// CPMUpdateDocumentsIn() request directs the server to index the specified path
        /// </summary>
        /// <param name="_flag"></param>
        /// <param name="_fRootPath"></param>
        public void CPMUpdateDocumentsIn(uint _flag, uint _fRootPath)
        {
            byte[] updateDocumentsOutMessage = null;
            RequestSender sender = GetRequestSender(isClientConnected);

            string rootPath;
            //If set to 0x00000001, a path on which to perform the update is included in RootPath;
            //If set to 0x00000000, the update is to be performed on all indexed paths.
            if (_fRootPath == 0x00000001)
            {
                rootPath = Site.Properties["SharedPath"];
            }
            else
            {
                rootPath = "";
            }

            byte[] updateDocumentsInMessage = builder.GetCPMUpdateDocumentsIn(_flag, _fRootPath, rootPath);

            int bytesRead = sender.SendMessage(updateDocumentsInMessage,
                out updateDocumentsOutMessage);
            byte[] value = null;
            value = new byte[bytesRead];
            Array.Copy(updateDocumentsOutMessage, 0, value, 0, bytesRead);

            uint checkSum = GetCheckSumField(updateDocumentsInMessage);
            // Read the message Response           

            int startingIndex = 0;
            uint msgId = Helper.GetUInt(updateDocumentsOutMessage, ref startingIndex);
            uint errorCode = Helper.GetUInt(updateDocumentsOutMessage, ref startingIndex);

            if (errorCode == 0)
            {
                validator.ValidateUpdateDocumentsOut(value, checkSum);
            }
            CPMUpdateDocumentsOutResponse(errorCode);

        }

        /// <summary>
        /// This event is used to get the response from 
        /// CPMUpdateDocumentsIn request.
        /// </summary>
        public event CPMUpdateDocumentsOutResponseHandler
            CPMUpdateDocumentsOutResponse;
        #endregion
        #endregion



        #region Helper Methods


        /// <summary>
        /// Get server's OS version
        /// </summary>
        /// <param name="platform">Represent server's platform type</param>
        /// <returns>The call must return true which indicates success</returns>
        public bool GetServerPlatform(out SkuOsVersion platform)
        {
            platform = CheckSkuOsVersion();
            _platform = platform;

            return true;
        }

        #region Check SKU OS Version

        /// <summary>
        /// Check SKU version.
        /// </summary>
        /// <returns>SKU version</returns>
        private SkuOsVersion CheckSkuOsVersion()
        {
            string skuOsVersion = wspTestSite.Properties.Get("ServerOSVersion");

            if (skuOsVersion.ToLower() != "NonWin".ToLower() &&
                skuOsVersion.ToLower() != "W2K3".ToLower() &&
                skuOsVersion.ToLower() != "W2K8".ToLower() &&
                skuOsVersion.ToLower() != "Win7".ToLower())
            {
                wspTestSite.Assert.Fail("Unknown SKU OS version: {0}", skuOsVersion);
            }

            switch (skuOsVersion.ToLower())
            {
                case "nonwin":
                    Properties.SkuOsVersion = SkuOsVersion.NonWindows;
                    break;
                case "w2k3":
                    Properties.SkuOsVersion = SkuOsVersion.Win2K3;
                    break;
                case "w2k8":
                    Properties.SkuOsVersion = SkuOsVersion.Win2K8;
                    break;
                case "win7":
                    Properties.SkuOsVersion = SkuOsVersion.Win2K8R2;
                    break;
                default:
                    break;
            }

            return Properties.SkuOsVersion;
        }

        #endregion

        /// <summary>
        /// Fetches Checksum field from a WSP message
        /// </summary>
        /// <param name="wspMessage">WSP message BLOB</param>
        /// <returns>Checksum</returns>
        private uint GetCheckSumField(byte[] wspMessage)
        {
            int index = 8;
            uint checkSumField = Helper.GetUInt(wspMessage, ref index);
            return checkSumField;
        }

        /// <summary>
        /// Retrieves the cursor associated with the client
        /// </summary>
        /// <param name="machineName">client machine name</param>
        /// <returns>cursor associated with the query</returns>
        public uint GetCursor(string machineName)
        {
            uint cursor = 0;
            if (cursorMap.ContainsKey(machineName))
            {
                cursor = cursorMap[machineName];
            }
            return cursor;
        }

        /// <summary>
        /// Returns a WspClient object to send message to Server 
        /// </summary>
        /// <param name="isclientConnected">true if the client is already Connected through CPMConnectIn request</param>
        /// <returns>WspClient</returns>
        private WspClient GetClient(bool isclientConnected)
        {
            if (isclientConnected)
            {
                return connectedClients[clientMachineName];
            }
            else
            {
                return defaultClient;
            }
        }

        /// <summary>
        /// Returns a RequestSender object to send message to Server 
        /// </summary>
        /// <param name="isclientConnected">true if the client is already
        /// Connected through CPMConnectIn request</param>
        /// <returns>RequestSender</returns>
        private RequestSender GetRequestSender(bool isclientConnected)
        {
            if (isclientConnected)
            {
                return connectedClients[clientMachineName].sender;
            }
            else
            {
                return defaultSender;
            }
        }

        /// <summary>
        /// Returns the Offset to be used by protocol client and server
        /// </summary>
        /// <returns>Offset to be used (either 64 or 32)</returns>
        private uint GetOffsetUsed()
        {
            uint returnValue = 0;
            uint clientOffset
                = Convert.ToUInt32(wspTestSite.Properties["ClientOffset"]);
            uint serverOffset
                = Convert.ToUInt32(wspTestSite.Properties["ServerOffset"]);
            if (serverOffset == Constant.OFFSET_64
                && clientOffset == Constant.OFFSET_64)
            {
                returnValue = Constant.OFFSET_64;
            }
            else
            {
                returnValue = Constant.OFFSET_32;
            }
            return returnValue;
        }

        /// <summary>
        /// Returns the WspMessageHeader by the adapter context.
        /// </summary>
        /// <param name="_msg">The original _msg value in header.</param>
        /// <param name="_status">The original _status value in header.</param>
        /// <param name="_ulChecksum">The orignial _ulChecksum value in header.</param>
        /// <returns>WspMessageHeader to be used.</returns>
        private WspMessageHeader? GetWspMessageHeader(WspMessageHeader_msg_Values _msg, uint _status, uint _ulChecksum)
        {
            WspMessageHeader? header = null;
            
            if (sendInvalidMsg)
            {
                header = new WspMessageHeader
                {
                    _msg = WspMessageHeader_msg_Values.Invalid,
                    _status = _status,
                    _ulChecksum = _ulChecksum
                };
            }
            
            if (sendInvalidStatus)
            {
                header = new WspMessageHeader
                {
                    _msg = _msg,
                    _status = 0xFFFFFFFFU,
                    _ulChecksum = _ulChecksum
                };
            }
            
            if (sendInvalidUlChecksum)
            {
                header = new WspMessageHeader
                {
                    _msg = _msg,
                    _status = _status,
                    _ulChecksum = 1
                };
                calculateChecksum = false;
            }

            return header;
        }
        #endregion
    }
}

