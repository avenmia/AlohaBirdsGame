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
    /// The OAuth user token information that can be used by clients, in exchange for user credentials, to access OAuth token-secured ArcGIS content and services.
    /// </summary>
    /// <since>1.1.0</since>
    [StructLayout(LayoutKind.Sequential)]
    public partial class ArcGISOAuthUserTokenInfo
    {
        #region Properties
        /// <summary>
        /// The access token to be used to make an authenticated request.
        /// </summary>
        /// <since>1.1.0</since>
        public string AccessToken
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_OAuthUserTokenInfo_getAccessToken(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return Unity.Convert.FromArcGISString(localResult);
            }
        }
        
        /// <summary>
        /// The access token expiration date.
        /// </summary>
        /// <since>1.1.0</since>
        public DateTimeOffset ExpirationDate
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_OAuthUserTokenInfo_getExpirationDate(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return Unity.Convert.FromArcGISDateTime(localResult);
            }
        }
        
        /// <summary>
        /// True if the token must be passed over HTTPS, false otherwise.
        /// </summary>
        /// <since>1.1.0</since>
        public bool IsSSLRequired
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_OAuthUserTokenInfo_getIsSSLRequired(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return localResult;
            }
        }
        
        /// <summary>
        /// The refresh token to be used to refresh the access token.
        /// </summary>
        /// <since>1.1.0</since>
        public string RefreshToken
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_OAuthUserTokenInfo_getRefreshToken(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return Unity.Convert.FromArcGISString(localResult);
            }
        }
        
        /// <summary>
        /// The date that the refresh token should be exchanged.
        /// </summary>
        /// <since>1.1.0</since>
        public DateTimeOffset RefreshTokenExchangeDate
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_OAuthUserTokenInfo_getRefreshTokenExchangeDate(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return Unity.Convert.FromArcGISDateTime(localResult);
            }
        }
        
        /// <summary>
        /// The date that the refresh token expires.
        /// </summary>
        /// <remarks>
        /// This is available only for ArcGIS.com and ArcGIS Enterprise version 10.9.1 and above.
        /// </remarks>
        /// <since>1.1.0</since>
        public DateTimeOffset RefreshTokenExpirationDate
        {
            get
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                var localResult = PInvoke.RT_OAuthUserTokenInfo_getRefreshTokenExpirationDate(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
                
                return Unity.Convert.FromArcGISDateTime(localResult);
            }
        }
        #endregion // Properties
        
        #region Methods
        /// <summary>
        /// Tests if the two <see cref="GameEngine.Authentication.ArcGISOAuthUserTokenInfo">ArcGISOAuthUserTokenInfo</see> objects are equal.
        /// </summary>
        /// <param name="other">The token information to compare to.</param>
        /// <returns>
        /// True if the comparison succeeds and the objects are equal, false otherwise.
        /// </returns>
        /// <since>1.1.0</since>
        public bool Equals(ArcGISOAuthUserTokenInfo other)
        {
            var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
            
            var localOther = other.Handle;
            
            var localResult = PInvoke.RT_OAuthUserTokenInfo_equals(Handle, localOther, errorHandler);
            
            Unity.ArcGISErrorManager.CheckError(errorHandler);
            
            return localResult;
        }
        #endregion // Methods
        
        #region Internal Members
        internal ArcGISOAuthUserTokenInfo(IntPtr handle) => Handle = handle;
        
        ~ArcGISOAuthUserTokenInfo()
        {
            if (Handle != IntPtr.Zero)
            {
                var errorHandler = Unity.ArcGISErrorManager.CreateHandler();
                
                PInvoke.RT_OAuthUserTokenInfo_destroy(Handle, errorHandler);
                
                Unity.ArcGISErrorManager.CheckError(errorHandler);
            }
        }
        
        internal IntPtr Handle { get; set; }
        
        public static implicit operator bool(ArcGISOAuthUserTokenInfo other)
        {
            return other != null && other.Handle != IntPtr.Zero;
        }
        #endregion // Internal Members
    }
    
    internal static partial class PInvoke
    {
        #region P-Invoke Declarations
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_OAuthUserTokenInfo_getAccessToken(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_OAuthUserTokenInfo_getExpirationDate(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool RT_OAuthUserTokenInfo_getIsSSLRequired(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_OAuthUserTokenInfo_getRefreshToken(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_OAuthUserTokenInfo_getRefreshTokenExchangeDate(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern IntPtr RT_OAuthUserTokenInfo_getRefreshTokenExpirationDate(IntPtr handle, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool RT_OAuthUserTokenInfo_equals(IntPtr handle, IntPtr other, IntPtr errorHandler);
        
        [DllImport(Unity.Interop.Dll)]
        internal static extern void RT_OAuthUserTokenInfo_destroy(IntPtr handle, IntPtr errorHandle);
        #endregion // P-Invoke Declarations
    }
}