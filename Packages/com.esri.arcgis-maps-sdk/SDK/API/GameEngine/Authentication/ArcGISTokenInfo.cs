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
    /// A type that has information about an ArcGIS authentication token.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal partial class ArcGISTokenInfo
    {
        #region Properties
        /// <summary>
        /// The access token string.
        /// </summary>
        internal string AccessToken
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_ArcGISTokenInfo_getAccessToken(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return Unity.Convert.FromArcGISString(localResult);
            }
        }
        
        /// <summary>
        /// The token expiration date.
        /// </summary>
        internal DateTimeOffset ExpirationDate
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_ArcGISTokenInfo_getExpirationDate(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return Unity.Convert.FromArcGISDateTime(localResult);
            }
        }
        
        /// <summary>
        /// A Boolean value that indicates whether the token must be passed over HTTPS.
        /// </summary>
        internal bool IsSSLRequired
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_ArcGISTokenInfo_getIsSSLRequired(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return localResult;
            }
        }
        #endregion // Properties
        
        #region Methods
        /// <summary>
        /// Tests if this object is equal to a second <see cref="GameEngine.Authentication.ArcGISTokenInfo">ArcGISTokenInfo</see> object.
        /// </summary>
        /// <param name="other">The second object.</param>
        /// <returns>
        /// True if the comparison succeeds and the objects are equal, false otherwise.
        /// </returns>
        internal bool Equals(ArcGISTokenInfo other)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localOther = other.Handle;
            
            var localResult = PInvoke.RT_ArcGISTokenInfo_equals(Handle, localOther, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            return localResult;
        }
        
        /// <summary>
        /// Indicates whether or not the access token has expired.
        /// </summary>
        /// <returns>
        /// True if the token's expiration date has passed, otherwise false.
        /// </returns>
        internal bool IsExpired()
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localResult = PInvoke.RT_ArcGISTokenInfo_isExpired(Handle, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            return localResult;
        }
        #endregion // Methods
        
        #region Internal Members
        internal ArcGISTokenInfo(IntPtr handle) => Handle = handle;
        
        ~ArcGISTokenInfo()
        {
            if (Handle != IntPtr.Zero)
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                PInvoke.RT_ArcGISTokenInfo_destroy(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
            }
        }
        
        internal IntPtr Handle { get; set; }
        
        public static implicit operator bool(ArcGISTokenInfo other)
        {
            return other != null && other.Handle != IntPtr.Zero;
        }
        #endregion // Internal Members
    }
    
    internal static partial class PInvoke
    {
        #region P-Invoke Declarations
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_ArcGISTokenInfo_getAccessToken(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_ArcGISTokenInfo_getExpirationDate(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool RT_ArcGISTokenInfo_getIsSSLRequired(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool RT_ArcGISTokenInfo_equals(IntPtr handle, IntPtr other, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool RT_ArcGISTokenInfo_isExpired(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern void RT_ArcGISTokenInfo_destroy(IntPtr handle, IntPtr errorHandle);
        #endregion // P-Invoke Declarations
    }
}