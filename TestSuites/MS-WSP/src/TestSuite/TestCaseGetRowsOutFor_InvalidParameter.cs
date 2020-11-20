// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1434
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.Protocols.TestSuites.WspTS
{
    using Microsoft.Protocols.TestTools;
    using Microsoft.Protocols.TestTools.Messages.Runtime;
    using System;


    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute()]
    public partial class TestCaseGetRowsOutFor_InvalidParameter : WspModelTestBase
    {

        public TestCaseGetRowsOutFor_InvalidParameter()
        {
            this.SetSwitch("generatedtestpath", "..\\\\TestSuite");
            this.SetSwitch("generatedtestnamespace", "Microsoft.Protocols.TestSuites.WspTS");
            this.SetSwitch("graphtimeout", "1000");
            this.SetSwitch("statebound", "-1");
            this.SetSwitch("stepbound", "6000");
            this.SetSwitch("pathbound", "32");
            this.SetSwitch("stepsperstatebound", "1024");
        }

        #region Expect Delegates
        public delegate void CPMConnectOutResponseDelegate1(uint errorCode);

        public delegate void CPMSetBindingsInResponseDelegate1(uint errorCode);

        public delegate void CPMGetRowsOutDelegate1(uint errorCode);

        public delegate void GetServerPlatformDelegate1(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.SkuOsVersion platform, bool @return);

        public delegate void CPMCreateQueryOutResponseDelegate1(uint errorCode);

        public delegate void CPMGetQueryStatusOutResponseDelegate1(uint errorCode);

        public delegate void CPMGetQueryStatusExOutResponseDelegate1(uint errorCode);

        public delegate void CPMRatioFinishedOutResponseDelegate1(uint errorCode);

        public delegate void CPMFetchValueOutResponseDelegate1(uint errorCode);

        public delegate void CPMCiStateInOutResponseDelegate1(uint errorCode);

        public delegate void CPMSendNotifyOutResponseDelegate1(uint errorCode);

        public delegate void CPMFreeCursorOutResponseDelegate1(uint errorCode);

        public delegate void CPMFindIndicesOutResponseDelegate1(uint errorCode);

        public delegate void CPMGetRowsetNotifyOutResponseDelegate1(uint errorCode);

        public delegate void CPMGetScopeStatisticsOutResponseDelegate1(uint errorCode);

        public delegate void CPMSetScopePrioritizationOutResponseDelegate1(uint errorCode);

        public delegate void CPMUpdateDocumentsOutResponseDelegate1(uint errorCode);
        #endregion

        #region Event Metadata
        static System.Reflection.MethodBase CPMConnectInRequestInfo = TestManagerHelpers.GetMethodInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMConnectInRequest");

        static System.Reflection.EventInfo CPMConnectOutResponseInfo = TestManagerHelpers.GetEventInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMConnectOutResponse");

        static System.Reflection.MethodBase CPMSetBindingsInInfo = TestManagerHelpers.GetMethodInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMSetBindingsIn", typeof(bool), typeof(bool));

        static System.Reflection.EventInfo CPMSetBindingsInResponseInfo = TestManagerHelpers.GetEventInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMSetBindingsInResponse");

        static System.Reflection.MethodBase CPMGetRowsInInfo = TestManagerHelpers.GetMethodInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMGetRowsIn", typeof(bool));

        static System.Reflection.EventInfo CPMGetRowsOutInfo = TestManagerHelpers.GetEventInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMGetRowsOut");

        static System.Reflection.MethodBase GetServerPlatformInfo = TestManagerHelpers.GetMethodInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "GetServerPlatform", typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.SkuOsVersion).MakeByRefType());

        static System.Reflection.MethodBase CPMCreateQueryInInfo = TestManagerHelpers.GetMethodInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMCreateQueryIn", typeof(bool));

        static System.Reflection.MethodBase CPMGetQueryStatusInInfo = TestManagerHelpers.GetMethodInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMGetQueryStatusIn", typeof(bool));

        static System.Reflection.MethodBase CPMGetQueryStatusExInInfo = TestManagerHelpers.GetMethodInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMGetQueryStatusExIn", typeof(bool));

        static System.Reflection.MethodBase CPMRatioFinishedInInfo = TestManagerHelpers.GetMethodInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMRatioFinishedIn", typeof(bool));

        static System.Reflection.MethodBase CPMFetchValueInInfo = TestManagerHelpers.GetMethodInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMFetchValueIn", typeof(bool));

        static System.Reflection.MethodBase CPMCiStateInOutInfo = TestManagerHelpers.GetMethodInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMCiStateInOut");

        static System.Reflection.MethodBase CPMGetNotifyInfo = TestManagerHelpers.GetMethodInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMGetNotify", typeof(bool));

        static System.Reflection.MethodBase CPMFreeCursorInInfo = TestManagerHelpers.GetMethodInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMFreeCursorIn", typeof(bool));

        static System.Reflection.MethodBase CPMDisconnectInfo = TestManagerHelpers.GetMethodInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMDisconnect");

        static System.Reflection.MethodBase CPMFindIndicesInInfo = TestManagerHelpers.GetMethodInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMFindIndicesIn", typeof(bool));

        static System.Reflection.MethodBase CPMGetRowsetNotifyInInfo = TestManagerHelpers.GetMethodInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMGetRowsetNotifyIn", typeof(int), typeof(bool));

        static System.Reflection.MethodBase CPMSetScopePrioritizationInInfo = TestManagerHelpers.GetMethodInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMSetScopePrioritizationIn", typeof(uint));

        static System.Reflection.MethodBase CPMGetScopeStatisticsInInfo = TestManagerHelpers.GetMethodInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMGetScopeStatisticsIn");

        static System.Reflection.EventInfo CPMCreateQueryOutResponseInfo = TestManagerHelpers.GetEventInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMCreateQueryOutResponse");

        static System.Reflection.EventInfo CPMGetQueryStatusOutResponseInfo = TestManagerHelpers.GetEventInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMGetQueryStatusOutResponse");

        static System.Reflection.EventInfo CPMGetQueryStatusExOutResponseInfo = TestManagerHelpers.GetEventInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMGetQueryStatusExOutResponse");

        static System.Reflection.EventInfo CPMRatioFinishedOutResponseInfo = TestManagerHelpers.GetEventInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMRatioFinishedOutResponse");

        static System.Reflection.EventInfo CPMFetchValueOutResponseInfo = TestManagerHelpers.GetEventInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMFetchValueOutResponse");

        static System.Reflection.EventInfo CPMCiStateInOutResponseInfo = TestManagerHelpers.GetEventInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMCiStateInOutResponse");

        static System.Reflection.EventInfo CPMSendNotifyOutResponseInfo = TestManagerHelpers.GetEventInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMSendNotifyOutResponse");

        static System.Reflection.EventInfo CPMFreeCursorOutResponseInfo = TestManagerHelpers.GetEventInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMFreeCursorOutResponse");

        static System.Reflection.EventInfo CPMFindIndicesOutResponseInfo = TestManagerHelpers.GetEventInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMFindIndicesOutResponse");

        static System.Reflection.EventInfo CPMGetRowsetNotifyOutResponseInfo = TestManagerHelpers.GetEventInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMGetRowsetNotifyOutResponse");

        static System.Reflection.EventInfo CPMGetScopeStatisticsOutResponseInfo = TestManagerHelpers.GetEventInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMGetScopeStatisticsOutResponse");

        static System.Reflection.EventInfo CPMSetScopePrioritizationOutResponseInfo = TestManagerHelpers.GetEventInfo(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter), "CPMSetScopePrioritizationOutResponse");

        #endregion

        #region Adapter Instances
        private Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter IWspAdapterInstance;
        #endregion

        #region Class Initialization and Cleanup
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
        public static void ClassInitialize(Microsoft.VisualStudio.TestTools.UnitTesting.TestContext context)
        {
            PtfTestClassBase.Initialize(context);
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute()]
        public static void ClassCleanup()
        {
            PtfTestClassBase.Cleanup();
        }
        #endregion

        #region Test Initialization and Cleanup
        protected override void TestInitialize()
        {
            this.InitializeTestManager();
            this.IWspAdapterInstance = ((Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter)(this.GetAdapter(typeof(Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP.Adapter.IWspAdapter))));
            this.Subscribe(CPMConnectOutResponseInfo, this.IWspAdapterInstance);
            this.Subscribe(CPMSetBindingsInResponseInfo, this.IWspAdapterInstance);
            this.Subscribe(CPMGetRowsOutInfo, this.IWspAdapterInstance);
            this.Subscribe(CPMCreateQueryOutResponseInfo, this.IWspAdapterInstance);
            this.Subscribe(CPMGetQueryStatusOutResponseInfo, this.IWspAdapterInstance);
            this.Subscribe(CPMGetQueryStatusExOutResponseInfo, this.IWspAdapterInstance);
            this.Subscribe(CPMRatioFinishedOutResponseInfo, this.IWspAdapterInstance);
            this.Subscribe(CPMFetchValueOutResponseInfo, this.IWspAdapterInstance);
            this.Subscribe(CPMCiStateInOutResponseInfo, this.IWspAdapterInstance);
            this.Subscribe(CPMSendNotifyOutResponseInfo, this.IWspAdapterInstance);
            this.Subscribe(CPMFreeCursorOutResponseInfo, this.IWspAdapterInstance);
            this.Subscribe(CPMFindIndicesOutResponseInfo, this.IWspAdapterInstance);
            this.Subscribe(CPMGetRowsetNotifyOutResponseInfo, this.IWspAdapterInstance);
            this.Subscribe(CPMGetScopeStatisticsOutResponseInfo, this.IWspAdapterInstance);
            this.Subscribe(CPMSetScopePrioritizationOutResponseInfo, this.IWspAdapterInstance);
        }

        protected override void TestCleanup()
        {
            base.TestCleanup();
            this.CleanupTestManager();
        }
        #endregion

        #region Test Starting in S0
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("MS-WSP_R592, MS-WSP_R647, MS-WSP_R651, MS-WSP_R653, MS-WSP_R654, MS-WSP_R704, MS-" +
            "WSP_R743")]
        public virtual void TestCaseGetRowsOutFor_InvalidParameterS0()
        {
            this.Manager.BeginTest("TestCaseGetRowsOutFor_InvalidParameterS0");
            this.Manager.Comment("reaching state \'S0\'");
            this.Manager.Comment("executing step \'call CPMConnectInRequest()\'");
            this.IWspAdapterInstance.CPMConnectInRequest();
            this.Manager.Comment("reaching state \'S1\'");
            this.Manager.Comment("checking step \'return CPMConnectInRequest\'");
            this.Manager.Comment("reaching state \'S2\'");
            this.Manager.ExpectEvent(this.QuiescenceTimeout, true, new ExpectedEvent(TestCaseGetRowsOutFor_InvalidParameter.CPMConnectOutResponseInfo, null, new CPMConnectOutResponseDelegate1(this.TestCaseGetRowsOutFor_InvalidParameterS0CPMConnectOutResponseChecker)));
            this.Manager.Comment("reaching state \'S3\'");
            this.Manager.Comment("executing step \'call CPMSetBindingsIn(True,True)\'");
            this.IWspAdapterInstance.CPMSetBindingsIn(true, true);
            this.Manager.Comment("reaching state \'S4\'");
            this.Manager.Comment("checking step \'return CPMSetBindingsIn\'");
            this.Manager.Comment("reaching state \'S5\'");
            this.Manager.ExpectEvent(this.QuiescenceTimeout, true, new ExpectedEvent(TestCaseGetRowsOutFor_InvalidParameter.CPMSetBindingsInResponseInfo, null, new CPMSetBindingsInResponseDelegate1(this.TestCaseGetRowsOutFor_InvalidParameterS0CPMSetBindingsInResponseChecker)));
            this.Manager.Comment("reaching state \'S6\'");
            this.Manager.Comment("executing step \'call CPMGetRowsIn(True)\'");
            this.IWspAdapterInstance.CPMGetRowsIn(true);
            this.Manager.Comment("reaching state \'S7\'");
            this.Manager.Comment("checking step \'return CPMGetRowsIn\'");
            this.Manager.Comment("reaching state \'S8\'");
            this.Manager.ExpectEvent(this.QuiescenceTimeout, true, new ExpectedEvent(TestCaseGetRowsOutFor_InvalidParameter.CPMGetRowsOutInfo, null, new CPMGetRowsOutDelegate1(this.TestCaseGetRowsOutFor_InvalidParameterS0CPMGetRowsOutChecker)));
            this.Manager.Comment("reaching state \'S9\'");
            this.Manager.EndTest();
        }

        private void TestCaseGetRowsOutFor_InvalidParameterS0CPMConnectOutResponseChecker(uint errorCode)
        {
            this.Manager.Comment("checking step \'event CPMConnectOutResponse(0)\'");
            this.Manager.Assert((errorCode == 0), String.Format("expected \'0\', actual \'{0}\' (errorCode of CPMConnectOutResponse, state S2)", errorCode));
            this.Manager.Checkpoint("MS-WSP_R592");
            this.Manager.Checkpoint("MS-WSP_R647");
            this.Manager.Checkpoint("MS-WSP_R651");
            this.Manager.Checkpoint("MS-WSP_R653");
            this.Manager.Checkpoint("MS-WSP_R654");
        }

        private void TestCaseGetRowsOutFor_InvalidParameterS0CPMSetBindingsInResponseChecker(uint errorCode)
        {
            this.Manager.Comment("checking step \'event CPMSetBindingsInResponse(3221225485)\'");
            this.Manager.Assert((errorCode == 3221225485), String.Format("expected \'3221225485\', actual \'{0}\' (errorCode of CPMSetBindingsInResponse, state" +
                        " S5)", errorCode));
            this.Manager.Checkpoint("MS-WSP_R743");
        }

        private void TestCaseGetRowsOutFor_InvalidParameterS0CPMGetRowsOutChecker(uint errorCode)
        {
            this.Manager.Comment("checking step \'event CPMGetRowsOut(3221225485)\'");
            this.Manager.Assert((errorCode == 3221225485), String.Format("expected \'3221225485\', actual \'{0}\' (errorCode of CPMGetRowsOut, state S8)", errorCode));
            this.Manager.Checkpoint("MS-WSP_R704");
        }
        #endregion
    }
}
