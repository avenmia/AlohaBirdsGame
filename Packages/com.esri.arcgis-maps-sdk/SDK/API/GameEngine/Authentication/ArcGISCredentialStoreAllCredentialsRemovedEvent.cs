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
    /// A callback invoked when all credentials are removed from the store.
    /// </summary>
    internal delegate void ArcGISCredentialStoreAllCredentialsRemovedEvent();
    
    internal delegate void ArcGISCredentialStoreAllCredentialsRemovedEventInternal(IntPtr userData);
    
    internal class ArcGISCredentialStoreAllCredentialsRemovedEventHandler : Unity.ArcGISEventHandler<ArcGISCredentialStoreAllCredentialsRemovedEvent>
    {
        [Unity.MonoPInvokeCallback(typeof(ArcGISCredentialStoreAllCredentialsRemovedEventInternal))]
        internal static void HandlerFunction(IntPtr userData)
        {
            if (userData == IntPtr.Zero)
            {
                return;
            }
            
            var callbackObject = (ArcGISCredentialStoreAllCredentialsRemovedEventHandler)((GCHandle)userData).Target;
            
            if (callbackObject == null)
            {
                return;
            }
            
            var callback = callbackObject.m_delegate;
            
            if (callback == null)
            {
                return;
            }
            
            callback();
        }
    }
}