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
    /// An object that represents an inquiry of whether a portal is network secured.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal partial class ArcGISPortalNetworkSecuredInquiry
    {
        #region Properties
        /// <summary>
        /// The URL of the portal to check.
        /// </summary>
        internal string PortalURL
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_PortalNetworkSecuredInquiry_getPortalURL(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return Unity.Convert.FromArcGISString(localResult);
            }
        }
        #endregion // Properties
        
        #region Methods
        /// <summary>
        /// Handles the inquiry that indicates if the portal is network secured.
        /// </summary>
        /// <param name="isPortalNetworkSecured">Flag that indicates if the portal is network secured.</param>
        internal void Respond(bool isPortalNetworkSecured)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            PInvoke.RT_PortalNetworkSecuredInquiry_respond(Handle, isPortalNetworkSecured, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
        }
        
        /// <summary>
        /// Handles an error that occurred at the platform API level during the inquiry.
        /// </summary>
        /// <param name="platformAPIError">The platform API error that was encountered during the inquiry.</param>
        internal void Respond(Standard.ArcGISClientReference platformAPIError)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localPlatformAPIError = platformAPIError.Handle;
            
            PInvoke.RT_PortalNetworkSecuredInquiry_respondWithPlatformAPIError(Handle, localPlatformAPIError, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
        }
        #endregion // Methods
        
        #region Internal Members
        internal ArcGISPortalNetworkSecuredInquiry(IntPtr handle) => Handle = handle;
        
        ~ArcGISPortalNetworkSecuredInquiry()
        {
            if (Handle != IntPtr.Zero)
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                PInvoke.RT_PortalNetworkSecuredInquiry_destroy(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
            }
        }
        
        internal IntPtr Handle { get; set; }
        
        public static implicit operator bool(ArcGISPortalNetworkSecuredInquiry other)
        {
            return other != null && other.Handle != IntPtr.Zero;
        }
        #endregion // Internal Members
    }
    
    internal static partial class PInvoke
    {
        #region P-Invoke Declarations
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_PortalNetworkSecuredInquiry_getPortalURL(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern void RT_PortalNetworkSecuredInquiry_respond(IntPtr handle, [MarshalAs(UnmanagedType.I1)]bool isPortalNetworkSecured, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern void RT_PortalNetworkSecuredInquiry_respondWithPlatformAPIError(IntPtr handle, IntPtr platformAPIError, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern void RT_PortalNetworkSecuredInquiry_destroy(IntPtr handle, IntPtr errorHandle);
        #endregion // P-Invoke Declarations
    }
}