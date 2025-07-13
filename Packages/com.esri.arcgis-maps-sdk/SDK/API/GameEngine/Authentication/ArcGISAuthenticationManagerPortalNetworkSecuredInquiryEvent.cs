// COPYRIGHT 1995-2025 ESRI
// TRADE SECRETS: ESRI PROPRIETARY AND CONFIDENTIAL
// Unpublished material - all rights reserved under the
// Copyright Laws of the United States and applicable international
// laws, treaties, and conventions.
//
// For additional information, contact:
// Attn: Contracts and Legal Department
// Environmental Systems Research Institute, Inc.
// 380 New York Street
// Redlands, California 92373
// USA
//
// email: legal@esri.com
using System.Runtime.InteropServices;
using System;

namespace Esri.GameEngine.Authentication
{
    /// <summary>
    /// A callback invoked when a portal network secured inquiry is issued.
    /// </summary>
    internal delegate void ArcGISAuthenticationManagerPortalNetworkSecuredInquiryEvent(ArcGISPortalNetworkSecuredInquiry portalNetworkSecuredInquiry);
    
    internal delegate void ArcGISAuthenticationManagerPortalNetworkSecuredInquiryEventInternal(IntPtr userData, IntPtr portalNetworkSecuredInquiry);
    
    internal class ArcGISAuthenticationManagerPortalNetworkSecuredInquiryEventHandler : Unity.ArcGISEventHandler<ArcGISAuthenticationManagerPortalNetworkSecuredInquiryEvent>
    {
        [Unity.MonoPInvokeCallback(typeof(ArcGISAuthenticationManagerPortalNetworkSecuredInquiryEventInternal))]
        internal static void HandlerFunction(IntPtr userData, IntPtr portalNetworkSecuredInquiry)
        {
            if (userData == IntPtr.Zero)
            {
                return;
            }
            
            var callbackObject = (ArcGISAuthenticationManagerPortalNetworkSecuredInquiryEventHandler)((GCHandle)userData).Target;
            
            if (callbackObject == null)
            {
                return;
            }
            
            var callback = callbackObject.m_delegate;
            
            if (callback == null)
            {
                return;
            }
            
            ArcGISPortalNetworkSecuredInquiry localPortalNetworkSecuredInquiry = null;
            
            if (portalNetworkSecuredInquiry != IntPtr.Zero)
            {
                localPortalNetworkSecuredInquiry = new ArcGISPortalNetworkSecuredInquiry(portalNetworkSecuredInquiry);
            }
            
            callback(localPortalNetworkSecuredInquiry);
        }
    }
}