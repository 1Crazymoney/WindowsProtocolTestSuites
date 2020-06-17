﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Protocols.TestTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter;
using Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP;

namespace Microsoft.Protocols.TestSuites.WspTS
{
    [TestClass]
    public partial class CPMGetRowsTestCases : WspCommonTestBase
    {
        private WspAdapter wspAdapter;
        private const uint validRowsToTransfer = 40;
        private const uint validReadBuffer = 0x4000;

        private const uint invalidReadBuffer = 0x00004001; // The value MUST NOT exceed 0x00004000.
        private const uint invalidRowsWidth = 0;
        private const uint invalidCursor = 0;

        public enum ArgumentType
        {
            AllValid,
            InvalidCursor,
            InvalidRowWidth,
            InvalidReadBuffer
        }
        private ArgumentType argumentType;

        #region Test Initialize and Cleanup
        [ClassInitialize()]
        public static void ClassInitialize(TestContext testContext)
        {
            TestClassBase.Initialize(testContext);
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            TestClassBase.Cleanup();
        }
        #endregion

        #region Test Case Initialize and Cleanup
        protected override void TestInitialize()
        {
            base.TestInitialize();
            wspAdapter = new WspAdapter();
            wspAdapter.Initialize(this.Site);

            wspAdapter.CPMConnectOutResponse += EnsureSuccessfulCPMConnectOut;
            wspAdapter.CPMSetBindingsInResponse += EnsureSuccessfulCPMSetBindingsOut;
            wspAdapter.CPMCreateQueryOutResponse += EnsureSuccessfulCPMCreateQueryOut;

            wspAdapter.CPMGetRowsOut += CPMGetRowsOut;
        }

        protected override void TestCleanup()
        {
            base.TestCleanup();
        }
        #endregion

        #region Test Cases

        [TestMethod]
        [TestCategory("BVT")]
        [TestCategory("CPMGetRows")]
        [Description("This test case is designed to test the basic functionality of CPMGetRows.")]
        public void BVT_CPMGetRows()
        {
            argumentType = ArgumentType.AllValid;
            Site.Log.Add(LogEntryKind.TestStep, "Client sends CPMConnectIn and expects success.");
            wspAdapter.CPMConnectInRequest();

            Site.Log.Add(LogEntryKind.TestStep, "Client sends CPMCreateQueryIn and expects success.");
            wspAdapter.CPMCreateQueryIn(false);

            Site.Log.Add(LogEntryKind.TestStep, "Client sends CPMSetBindingsIn and expects success.");
            wspAdapter.CPMSetBindingsIn(true, true);

            Site.Log.Add(LogEntryKind.TestStep, "Client sends CPMGetRowsIn and expects success.");
            CPMGetRowsOut getRowsOut;
            wspAdapter.CPMGetRowsIn(
                wspAdapter.GetCursor(wspAdapter.clientMachineName),
                validRowsToTransfer,
                MessageBuilder.rowWidth,
                validReadBuffer,
                (uint)FetchType.ForwardOrder,
                (uint)RowSeekType.eRowSeekNext,
                out getRowsOut);


            Site.Assert.AreEqual((uint)13, getRowsOut._cRowsReturned, "The rows returned should be 13 since only 13 files out of 37 files have the content \"test\".");
            Site.Log.Add(LogEntryKind.TestStep, "The rows returned as below: ");

            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendLine();
            strBuilder.AppendLine("      System.ItemName;  System.ItemFolderNameDisplay");
            for (int i = 0; i < getRowsOut._cRowsReturned; i++)
            {
                strBuilder.Append(string.Format("Row {0}: ", i));
                for (int j = 0; j < getRowsOut.Rows[i].Columns.Length; j++)
                {
                    strBuilder.Append(getRowsOut.Rows[i].Columns[j].Data);
                    strBuilder.Append(";      ");
                }
                strBuilder.AppendLine();
            }

            Site.Log.Add(LogEntryKind.Debug, strBuilder.ToString());

        }

        [TestMethod]
        [TestCategory("CPMGetRows")]
        [Description("This test case is designed to verify the server response if invalid Cursor is sent in CPMGetRowsIn.")]
        public void CPMGetRows_InvalidCursor()
        {

            Site.Log.Add(LogEntryKind.TestStep, "Client sends CPMConnectIn and expects success.");
            wspAdapter.CPMConnectInRequest();

            Site.Log.Add(LogEntryKind.TestStep, "Client sends CPMCreateQueryIn and expects success.");
            wspAdapter.CPMCreateQueryIn(false);

            Site.Log.Add(LogEntryKind.TestStep, "Client sends CPMSetBindingsIn and expects success.");
            wspAdapter.CPMSetBindingsIn(true, true);

            Site.Log.Add(LogEntryKind.TestStep, "Client sends CPMGetRowsIn with invalid cursor and expects ERROR_INVALID_PARAMETER .");
            argumentType = ArgumentType.InvalidCursor;
            CPMGetRowsOut getRowsOut;
            wspAdapter.CPMGetRowsIn(
                invalidCursor,
                validRowsToTransfer,
                MessageBuilder.rowWidth,
                validReadBuffer,
                (uint)FetchType.ForwardOrder,
                (uint)RowSeekType.eRowSeekNext,
                out getRowsOut);
        }

        [TestMethod]
        [TestCategory("CPMGetRows")]
        [Description("This test case is designed to verify the server response if invalid RowWidth is sent in CPMGetRowsIn.")]
        public void CPMGetRows_InvalidRowWidth()
        {

            Site.Log.Add(LogEntryKind.TestStep, "Client sends CPMConnectIn and expects success.");
            wspAdapter.CPMConnectInRequest();

            Site.Log.Add(LogEntryKind.TestStep, "Client sends CPMCreateQueryIn and expects success.");
            wspAdapter.CPMCreateQueryIn(false);

            Site.Log.Add(LogEntryKind.TestStep, "Client sends CPMSetBindingsIn and expects success.");
            wspAdapter.CPMSetBindingsIn(true, true);

            Site.Log.Add(LogEntryKind.TestStep, "Client sends CPMGetRowsIn with invalid RowWidth and expects ERROR_INVALID_PARAMETER .");
            argumentType = ArgumentType.InvalidRowWidth;
            CPMGetRowsOut getRowsOut;
            wspAdapter.CPMGetRowsIn(
                wspAdapter.GetCursor(wspAdapter.clientMachineName),
                validRowsToTransfer,
                invalidRowsWidth,
                validReadBuffer,
                (uint)FetchType.ForwardOrder,
                (uint)RowSeekType.eRowSeekNext,
                out getRowsOut);
        }

        [TestMethod]
        [TestCategory("CPMGetRows")]
        [Description("This test case is designed to verify the server response if invalid ReadBuffer is sent in CPMGetRowsIn.")]
        public void CPMGetRows_InvalidReadBuffer()
        {

            Site.Log.Add(LogEntryKind.TestStep, "Client sends CPMConnectIn and expects success.");
            wspAdapter.CPMConnectInRequest();

            Site.Log.Add(LogEntryKind.TestStep, "Client sends CPMCreateQueryIn and expects success.");
            wspAdapter.CPMCreateQueryIn(false);

            Site.Log.Add(LogEntryKind.TestStep, "Client sends CPMSetBindingsIn and expects success.");
            wspAdapter.CPMSetBindingsIn(true, true);

            Site.Log.Add(LogEntryKind.TestStep, "Client sends CPMGetRowsIn with invalid ReadBuffer and expects STATUS_INVALID_PARAMETER .");
            argumentType = ArgumentType.InvalidReadBuffer;
            CPMGetRowsOut getRowsOut;
            wspAdapter.CPMGetRowsIn(
                wspAdapter.GetCursor(wspAdapter.clientMachineName),
                validRowsToTransfer,
                MessageBuilder.rowWidth,
                invalidReadBuffer,
                (uint)FetchType.ForwardOrder,
                (uint)RowSeekType.eRowSeekNext,
                out getRowsOut);
        }

        #endregion

        private void CPMGetRowsOut(uint errorCode)
        {
            switch (argumentType)
            {
                case ArgumentType.AllValid:
                    bool succeed = errorCode == (uint)WspErrorCode.SUCCESS || errorCode == (uint)WspErrorCode.DB_S_ENDOFROWSET ? true : false;
                    Site.Assert.IsTrue(succeed, "Server should return succeed or DB_S_ENDOFROWSET for CPMGetRowsIn.");
                    break;
                case ArgumentType.InvalidCursor:
                    Site.Assert.AreEqual((uint)WspErrorCode.ERROR_INVALID_PARAMETER, errorCode, "Server should return ERROR_INVALID_PARAMETER if Cursor of CPMGetRowsIn is invalid.");
                    break;
                case ArgumentType.InvalidRowWidth:
                    Site.Assert.AreEqual((uint)WspErrorCode.STATUS_INVALID_PARAMETER, errorCode, "Server should return STATUS_INVALID_PARAMETER if RowWidth of CPMGetRowsIn is invalid.");
                    break;
                case ArgumentType.InvalidReadBuffer:
                    Site.Assert.AreEqual((uint)WspErrorCode.STATUS_INVALID_PARAMETER, errorCode, "Server should return STATUS_INVALID_PARAMETER if ReadBuffer of CPMGetRowsIn is invalid.");
                    break;

            }
        }
    }
}
