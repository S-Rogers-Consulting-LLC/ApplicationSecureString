using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Library.Extentions;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading;

namespace System.Security {
    /// <summary>
    /// ApplicationSecureString is a Direct replacement for String for all Results, Members, Properties, or as a Parameter.
    /// </summary>
    [Serializable]
    [CompilationRelaxations(CompilationRelaxations.NoStringInterning)]
    public sealed class ApplicationSecureString : ISerializable, IDisposable {
        #region Members
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SecureString TheSecureString;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        [DebuggerStepThrough]
        private ApplicationSecureString() {
            Thread.MemoryBarrier();
            TheSecureString = String.Empty.CreateSecureString();
        }

        /// <summary>
        /// Constructor using SecureString.
        /// </summary>
        /// <param name="argSecureString">SecureString to copy and wrap.</param>
        [DebuggerStepThrough]
        public ApplicationSecureString(SecureString argSecureString) {
            Thread.MemoryBarrier();
            TheSecureString = argSecureString.Copy();
        }

        /// <summary>
        /// Constructor using Char[].
        /// </summary>
        /// <param name="argCharacters">Char[] to encrypt.</param>
        [DebuggerStepThrough]
        public ApplicationSecureString(params Char[] argCharacters) {
            Thread.MemoryBarrier();
            TheSecureString = argCharacters.CreateSecureString();
        }

        /// <summary>
        /// Constructor using String.
        /// </summary>
        /// <param name="argString">String to encrypt.</param>
        [DebuggerStepThrough]
        public ApplicationSecureString(String argString) {
            Thread.MemoryBarrier();
            TheSecureString = argString.CreateSecureString();
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        [DebuggerStepThrough]
        ~ApplicationSecureString() {
            Dispose();
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Copies the string in this instance to a Unicode characters.
        /// </summary>
        /// <returns> A Unicode string whose elements are the individual characters of this
        ///     instance. If this instance is an empty string, the returned string is empty and
        ///     has a zero length.
        /// </returns>
        public string CreateUnsecuredString() { return TheSecureString.CreateUnencryptedString(); }

        /// <summary>
        ///     Copies the characters in this instance to a Unicode character array.
        /// </summary>
        /// <returns> A Unicode character array whose elements are the individual characters of this
        ///     instance. If this instance is an empty string, the returned array is empty and
        ///     has a zero length.
        /// </returns>
        public Char[] CreateUnsecuredCharacters() { return TheSecureString.CreateUnencryptedCharacters(); }

        /// <summary>
        /// Create StringDisposable() and apply the 'IDisposable' using(var stringDisposable = ApplicationSecureString.CreateStringDisposable()){ do stuff in here. }
        /// </summary>
        [DebuggerStepThrough]
        public StringDisposable CreateStringDisposable() {
            var unencryptedCharacters = TheSecureString.CreateUnencryptedCharacters();
            try {
                return new StringDisposable(unencryptedCharacters);
            } finally {
                unencryptedCharacters.ClearCharacters();
            }
        }
        #endregion

        #region Public Methods That Emulate String Methods
        /// <summary>
        /// Determines whether the specified System.Object is equal to the current System.Object. 
        /// However, we will be using the internal SecureString to determine equality of the content.     
        /// </summary>
        /// <param name="argObject">The System.Object to compare with the current System.Object</param>
        /// <returns>true if the specified System.Object is equal to the current System.Object; otherwise, False.</returns>
        [DebuggerStepThrough]
        public override bool Equals(System.Object argObject) {
            var applicationSecureString = argObject as ApplicationSecureString;
            if ((object)applicationSecureString == null)
                return false;

            return ApplicationSecureStringEqual(applicationSecureString, this);
        }

        /// <summary>
        /// Determines whether the specified ApplicationSecureString is equal to the current ApplicationSecureString. 
        /// However, we will be using the internal SecureString to determine equality of the content.     
        /// </summary>
        /// <param name="argApplicationSecureString">The ApplicationSecureStringt to compare with the current ApplicationSecureString</param>
        /// <returns>true if the specified System.Object is equal to the current System.Object; otherwise, False.</returns>
        [DebuggerStepThrough]
        public bool Equals(ApplicationSecureString argApplicationSecureString) { return ApplicationSecureStringEqual(argApplicationSecureString, this); }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the internal SecureString.</returns>
        [DebuggerStepThrough]
        public override int GetHashCode() { return TheSecureString.GetHashCode(); }

        /// <summary>
        ///  Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object.</returns>
        [DebuggerStepThrough]
        public override string ToString() {
            return TheSecureString.CreateUnencryptedString();
        }
        #endregion

        #region ISerializable
        /// <summary>
        ///  Reads from System.Runtime.Serialization.SerializationInfo the data needed to deserialize the target object.
        /// </summary>
        /// <param name="argSerializationInfo">The System.Runtime.Serialization.SerializationInfo to populate with data.</param>
        /// <param name="argStreamingContext">The destination (see System.Runtime.Serialization.StreamingContext) for this serialization.</param>
        [DebuggerStepThrough]
        private ApplicationSecureString(SerializationInfo argSerializationInfo, StreamingContext argStreamingContext) { SecureContext = (String)argSerializationInfo.GetValue(Wire.PropertyName(), typeof(String)); }

        /// <summary>
        ///  Populates a System.Runtime.Serialization.SerializationInfo with the data needed to serialize the target object.
        /// </summary>
        /// <param name="argSerializationInfo">The System.Runtime.Serialization.SerializationInfo to populate with data.</param>
        /// <param name="argStreamingContext">The destination (see System.Runtime.Serialization.StreamingContext) for this serialization.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        [DebuggerStepThrough]
        public void GetObjectData(SerializationInfo argSerializationInfo, StreamingContext argStreamingContext) { argSerializationInfo.AddValue(Wire.PropertyName(), SecureContext); }
        #endregion

        #region IDisposable
        /// <summary>
        /// Releases all resources used by the internal System.Security.SecureString object.
        /// </summary>
        [DebuggerStepThrough]
        public void Dispose() {
            Thread.MemoryBarrier();
            if (null == TheSecureString)
                return;

            TheSecureString.Dispose();
            TheSecureString = null;
        }
        #endregion

        #region Public Implicit Casting Operators
        /// <summary>
        /// Converts a String into a ApplicationSecureString.
        /// </summary>
        /// <param name="argString">String</param>
        [DebuggerStepThrough]
        public static implicit operator ApplicationSecureString(String argString) { return new ApplicationSecureString(argString); }

        /// <summary>
        /// Converts a Char[] into a ApplicationSecureString.
        /// </summary>
        /// <param name="argCharacters">Char[]</param>
        [DebuggerStepThrough]
        public static implicit operator ApplicationSecureString(Char[] argCharacters) { return new ApplicationSecureString(argCharacters); }

        /// <summary>
        /// Converts ApplicationSecureString into a String.
        /// </summary>
        /// <param name="argApplicationSecureString">ApplicationSecureString</param>
        [DebuggerStepThrough]
        public static implicit operator String(ApplicationSecureString argApplicationSecureString) { return argApplicationSecureString.CreateUnsecuredString(); }

        /// <summary>
        ///  Converts ApplicationSecureString into a Char[].
        /// </summary>
        /// <param name="argApplicationSecureString">ApplicationSecureString</param>
        [DebuggerStepThrough]
        public static implicit operator Char[] (ApplicationSecureString argApplicationSecureString) { return argApplicationSecureString.CreateUnsecuredCharacters(); }
        #endregion

        #region Public Override Operators
        /// <summary>
        ///     Determines whether two specified ApplicationSecureString have the same value.
        /// </summary>
        /// <param name="argApplicationSecureStringA">The first ApplicationSecureString to compare, or null</param>
        /// <param name="argApplicationSecureStringB">The second ApplicationSecureString to compare, or null</param>
        /// <returns>true if the value of argApplicationSecureStringA is the same as the value of argApplicationSecureStringB; otherwise, false.</returns>
        [DebuggerStepThrough]
        public static bool operator ==(ApplicationSecureString argApplicationSecureStringA, ApplicationSecureString argApplicationSecureStringB) {
            return ApplicationSecureStringEqual(argApplicationSecureStringA, argApplicationSecureStringB);
        }

        /// <summary>
        ///     Determines whether two specified strings have different values.        
        /// </summary>
        /// <param name="argApplicationSecureStringA">The first ApplicationSecureString to compare, or null.</param>
        /// <param name="argApplicationSecureStringB">The second ApplicationSecureString to compare, or null.</param>
        /// <returns>true if the value of argApplicationSecureStringA is different from the value of argApplicationSecureStringB; otherwise, false.</returns>
        [DebuggerStepThrough]
        public static bool operator !=(ApplicationSecureString argApplicationSecureStringA, ApplicationSecureString argApplicationSecureStringB) { return !(argApplicationSecureStringA == argApplicationSecureStringB); }
        #endregion

        #region Public Properties
        /// <summary>
        /// Provides an encrypted String that is Base 64 encoded for transport..
        /// </summary>
        public String SecureContext {
            get {
                var unencryptedCharacters = TheSecureString.CreateUnencryptedCharacters();
                try {
                    var wireEncryptedCharacters = Wire.Encrypt(unencryptedCharacters);
                    try {
                        return wireEncryptedCharacters.CreateString();
                    } finally {
                        wireEncryptedCharacters.ClearCharacters();
                    }
                } finally {
                    unencryptedCharacters.ClearCharacters();
                }
            }
            set {
                var characters = value.CreateCharacters();
                try {
                    var wireDecryptedCharacters = Wire.Decrypt(characters);
                    try {
                        if (null != TheSecureString)
                            TheSecureString.Dispose();
                        TheSecureString = wireDecryptedCharacters.CreateSecureString();
                    } finally {
                        wireDecryptedCharacters.ClearCharacters();
                    }
                } finally {
                    characters.ClearCharacters();
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        ///  Determines whether two specified ApplicationSecureString have the same value.
        /// </summary>
        /// <param name="argApplicationSecureStringA">>The first string to compare, or null.</param>
        /// <param name="argApplicationSecureStringB">The second string to compare, or null.</param>
        /// <returns>true if the value of argApplicationSecureStringA is the same as the value of argApplicationSecureStringB; otherwise, false.</returns>
        [DebuggerStepThrough]
        private static bool ApplicationSecureStringEqual(ApplicationSecureString argApplicationSecureStringA, ApplicationSecureString argApplicationSecureStringB) {
            if (System.Object.ReferenceEquals(argApplicationSecureStringA, argApplicationSecureStringB))
                return true;

            if (((object)argApplicationSecureStringA == null) || ((object)argApplicationSecureStringB == null))
                return false;

            using (var stringDisposableA = argApplicationSecureStringA.CreateStringDisposable())
            using (var stringDisposableB = argApplicationSecureStringB.CreateStringDisposable()) {
                return (stringDisposableA == stringDisposableB);
            }
        }
        #endregion

        #region Private Classes
        private static class Wire {
            #region Members
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private static readonly byte[] TheCryptoKey = new byte[24];
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private static readonly string ThePropertyName;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private static readonly byte[] TheIV = new byte[] { 00, 00, 00, 00, 00, 00, 00, 00 }; // Change to make your encrypt unique.
            #endregion

            #region Constructors
            [DebuggerStepThrough]
            static Wire() {
                var uniqueKey = Assembly.GetExecutingAssembly().GetType().GUID.ToString();
                var base64EncodedBytes = uniqueKey.CreateBase64EncodedString().CreateBytes();
                try {
                    var computedHash = SHA256.Create().ComputeHash(base64EncodedBytes, 0, 32);
                    try {
                        Array.Copy(computedHash, TheCryptoKey, TheCryptoKey.Length);
                    } finally {
                        computedHash.ClearBytes();
                    }
                } finally {
                    base64EncodedBytes.ClearBytes();
                }

                var characters = uniqueKey.CreateCharacters();
                try {
                    var wireEncryptedCharacters = Encrypt(characters);
                    try {
                        ThePropertyName = wireEncryptedCharacters.CreateBase64EncodedString();
                    } finally {
                        wireEncryptedCharacters.ClearCharacters();
                    }
                } finally {
                    characters.ClearCharacters();
                }
            }
            #endregion

            #region Public Methods
            [DebuggerStepThrough]
            public static string PropertyName() { return ThePropertyName; }

            [DebuggerStepThrough]
            public static Char[] Encrypt(Char[] argCharacters) {
                using (var tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider()) {
                    using (var cryptoTransform = tripleDESCryptoServiceProvider.CreateEncryptor(TheCryptoKey, TheIV)) {
                        var encryptArray = argCharacters.CreateBytes();
                        try {
                            var resultArray = cryptoTransform.TransformFinalBlock(encryptArray, 0, encryptArray.Length);
                            try {
                                return resultArray.CreateBase64EncodedCharacters();
                            } finally {
                                resultArray.ClearBytes();
                            }
                        } finally {
                            encryptArray.ClearBytes();
                        }
                    }
                }
            }

            [DebuggerStepThrough]
            public static Char[] Decrypt(Char[] argCharacters) {
                using (var tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider()) {
                    using (var cryptoTransform = tripleDESCryptoServiceProvider.CreateDecryptor(TheCryptoKey, TheIV)) {
                        var decryptByteArray = Convert.FromBase64CharArray(argCharacters, 0, argCharacters.Length);
                        try {
                            var resultByteArray = cryptoTransform.TransformFinalBlock(decryptByteArray, 0, decryptByteArray.Length);
                            try {
                                return resultByteArray.CreateCharacters();
                            } finally {
                                resultByteArray.ClearBytes();
                            }
                        } finally {
                            decryptByteArray.ClearBytes();
                        }
                    }
                }
            }
            #endregion
        }
        #endregion

        #region Public Classes
        /// <summary>
        /// Provides a wrapper around a pinned string with the IDisposable interface that will Zero the memory an release it when called
        /// </summary>
        [CompilationRelaxations(CompilationRelaxations.NoStringInterning)]
        public sealed class StringDisposable : IDisposable {
            #region Members
            private readonly GCHandle TheGCHandle;
            #endregion

            #region Constructors
            /// <summary>
            /// Blocks public creation.
            /// </summary>
            private StringDisposable() { UnsecuredString = string.Empty; }

            /// <summary>
            /// Creates an internal String that is pinned in memory so that It may be released on Dispose();
            /// </summary>
            /// <param name="argCharacters"></param>
            [SecurityCritical]
            [DebuggerStepThrough]
            [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
            public StringDisposable(Char[] argCharacters) {
                Thread.MemoryBarrier();
                UnsecuredString = string.Empty;

                if (null == argCharacters)
                    return;

                UnsecuredString = new String(argCharacters);
                if (String.IsNullOrEmpty(UnsecuredString))
                    return;

                var internedString = String.IsInterned(UnsecuredString);
                if (!String.IsNullOrEmpty(internedString))
                    return;

                TheGCHandle = GCHandle.Alloc(UnsecuredString, GCHandleType.Pinned);
            }

            /// <summary>
            /// Finalizer.
            /// </summary>
            ~StringDisposable() { Dispose(); }
            #endregion

            #region IDisposable
            /// <summary>
            /// Releases all resources used by the internal System.String object via ZeroMemory.
            /// </summary>   
            [SecurityCritical]
            [DebuggerStepThrough]
            [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
            public void Dispose() {
                lock (this) {
                    Thread.MemoryBarrier();
                    if (String.IsNullOrEmpty(UnsecuredString))
                        return;

                    var internedString = String.IsInterned(UnsecuredString);
                    if (!String.IsNullOrEmpty(internedString))
                        return;

                    try {
                        try {
                            if (TheGCHandle.IsAllocated) {
                                var addrOfPinnedObject = TheGCHandle.AddrOfPinnedObject();
                                var lengthOfPinnedObject = (Int32)UnsecuredString.Length * 2;
                                Marshal.Copy(EmptyWhiteSpace.GetAutoExpandingWhiteSpaceByteArray(lengthOfPinnedObject), 0, addrOfPinnedObject, lengthOfPinnedObject);
                            }
                        } finally {
                            if (TheGCHandle.IsAllocated)
                                TheGCHandle.Free();
                        }
                    } finally {
                        UnsecuredString = string.Empty;
                    }
                }
            }
            #endregion

            #region Public Properties
            /// <summary>
            /// A string pinned in memory until it is disposed.
            /// </summary>
            public String UnsecuredString { get; private set; }
            #endregion

            #region Public Implicit Casting Operators
            /// <summary>
            /// Presents StringDisposable as String.
            /// </summary>
            /// <param name="argStringDisposable">argStringDisposable</param>
            public static implicit operator String(StringDisposable argStringDisposable) { return argStringDisposable.UnsecuredString; }
            #endregion

            #region Public Methods That Emulate String Methods
            /// <summary>
            /// Determines whether the specified System.Object is equal to the current System.Object. 
            /// However, we will be using the internal String to determine equality of the content.     
            /// </summary>
            /// <param name="argObject">The System.Object to compare with the current System.Object</param>
            /// <returns>true if the specified System.Object is equal to the current System.Object; otherwise, False.</returns>
            public override bool Equals(System.Object argObject) {
                var stringDisposable = argObject as StringDisposable;
                if ((object)stringDisposable == null)
                    return false;

                return StringDisposableEqual(stringDisposable, this);
            }

            /// <summary>
            /// Determines whether the specified StringDisposable is equal to the current StringDisposable. 
            /// However, we will be using the internal SecureString to determine equality of the content.     
            /// </summary>
            /// <param name="argStringDisposable">The StringDisposable to compare with the current StringDisposable</param>
            /// <returns>true if the specified System.Object is equal to the current System.Object; otherwise, False.</returns>
            public bool Equals(StringDisposable argStringDisposable) { return StringDisposableEqual(argStringDisposable, this); }

            /// <summary>
            /// Serves as a hash function for a particular type.
            /// </summary>
            /// <returns>A hash code for the internal SecureString.</returns>
            public override int GetHashCode() { return UnsecuredString.GetHashCode(); }

            /// <summary>
            ///  Returns a string that represents the current object.
            /// </summary>
            /// <returns> A string that represents the current object.</returns>
            public override string ToString() { return UnsecuredString.ToString(); }
            #endregion

            #region Public Override Operators
            /// <summary>
            ///     Determines whether two specified StringDisposable have the same value.
            /// </summary>
            /// <param name="argStringDisposableA">The first StringDisposable to compare, or null</param>
            /// <param name="argStringDisposableB">The second StringDisposable to compare, or null</param>
            /// <returns>true if the value of argStringDisposableA is the same as the value of argStringDisposableB; otherwise, false.</returns>
            public static bool operator ==(StringDisposable argStringDisposableA, StringDisposable argStringDisposableB) {
                return StringDisposableEqual(argStringDisposableA, argStringDisposableB);
            }

            /// <summary>
            ///     Determines whether two specified StringDisposable have different values.        
            /// </summary>
            /// <param name="argStringDisposableA">The first StringDisposable to compare, or null.</param>
            /// <param name="argStringDisposableB">The second StringDisposable to compare, or null.</param>
            /// <returns>true if the value of argStringDisposableA is different from the value of argStringDisposableB; otherwise, false.</returns>
            public static bool operator !=(StringDisposable argStringDisposableA, StringDisposable argStringDisposableB) { return !(argStringDisposableA == argStringDisposableB); }
            #endregion

            #region Private Methods
            /// <summary>
            ///  Determines whether two specified StringDisposable have the same value.
            /// </summary>
            /// <param name="argStringDisposableA">>The first string to compare, or null.</param>
            /// <param name="argStringDisposableB">The second string to compare, or null.</param>
            /// <returns>true if the value of argStringDisposableA is the same as the value of argStringDisposableB; otherwise, false.</returns>
            private static bool StringDisposableEqual(StringDisposable argStringDisposableA, StringDisposable argStringDisposableB) {
                if (System.Object.ReferenceEquals(argStringDisposableA, argStringDisposableB))
                    return true;

                if (((object)argStringDisposableA == null) || ((object)argStringDisposableB == null))
                    return false;

                return (argStringDisposableA.UnsecuredString == argStringDisposableB.UnsecuredString);

            }
            #endregion

            #region Private Classes
            /// <summary>
            /// Provides empty byte array for overwriting strings.
            /// </summary>
            private static class EmptyWhiteSpace {
                #region Members
                private static Object TheSyncLock = new object();
                private static Byte[] TheEmptyByteArray = new Byte[0];
                #endregion

                /// <summary>
                /// Create default empty string.
                /// </summary>
                static EmptyWhiteSpace() {
                    GetAutoExpandingWhiteSpaceByteArray(1000);
                }

                /// <summary>
                /// Provides an empty byte array to overwrite string with sensitive data. 
                /// </summary>
                /// <param name="argLength">New max length of empty string if greater.</param>
                /// <returns></returns>
                public static Byte[] GetAutoExpandingWhiteSpaceByteArray(Int32 argLength) {
                    lock (TheSyncLock) {
                        if (TheEmptyByteArray.Length >= argLength)
                            return TheEmptyByteArray;

                        var length = argLength + 255;
                        var emptyByteArray = new Byte[length];
                        var index = 0;

                        for (index = 0; index < (emptyByteArray.Length - 1); index = index + 2) {
                            emptyByteArray[index] = 0;
                            emptyByteArray[index + 1] = (Byte)' ';
                        }

                        if (emptyByteArray.Length > TheEmptyByteArray.Length)
                            TheEmptyByteArray = emptyByteArray;
                        return TheEmptyByteArray;
                    }
                }
            }
            #endregion
        }
        #endregion
    }
}

#region ApplicationSecureStringExtensions
namespace System.Security.Library.Extentions {
    /// <summary>
    /// SecureString Extensions.
    /// </summary>
    [CompilationRelaxations(CompilationRelaxations.NoStringInterning)]
    internal static class ApplicationSecureStringExtensions {
        /// <summary>
        /// Creates a SecureString from a String.
        /// </summary>
        /// <param name="argString">String is used to create a SecureString.</param>
        /// <returns>SecureString</returns>
        [SecurityCritical]
        [DebuggerStepThrough]
        public static SecureString CreateSecureString(this String argString) {
            var charArray = argString.ToCharArray();
            try {
                return charArray.CreateSecureString();
            } finally {
                charArray.ClearCharacters();
            }
        }

        /// <summary>
        /// Creates a SecureString from a Char[].
        /// </summary>
        /// <param name="argCharacters">Char[] is used to create a SecureString.</param>
        /// <returns>SecureString</returns>
        [SecurityCritical]
        [DebuggerStepThrough]
        public static SecureString CreateSecureString(this Char[] argCharacters) {
            var secureString = new SecureString();
            foreach (var character in argCharacters)
                if (character != '\0')
                    secureString.AppendChar(character);
            secureString.MakeReadOnly();
            return secureString;
        }

        /// <summary>
        /// Creates a String from a SecureString.
        /// </summary>
        /// <param name="argSecureString">SecureString to be Unencrypted.</param>
        /// <returns>String</returns>
        [SecurityCritical]
        [DebuggerStepThrough]
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static String CreateUnencryptedString(this SecureString argSecureString) {
            var intPtr = Marshal.SecureStringToBSTR(argSecureString);
            try {
                var result = Marshal.PtrToStringBSTR(intPtr);
                return result;
            } finally {
                Marshal.ZeroFreeBSTR(intPtr);
            }
        }

        /// <summary>
        /// Creates a Char[] from a SecureString.
        /// </summary>
        /// <param name="argSecureString">SecureString to be Unencrypted.</param>
        /// <returns>Char[]</returns>
        [SecurityCritical]
        [DebuggerStepThrough]
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static Char[] CreateUnencryptedCharacters(this SecureString argSecureString) {
            var intPtr = Marshal.SecureStringToBSTR(argSecureString);
            try {
                var characters = new char[argSecureString.Length];
                Marshal.Copy(intPtr, characters, 0, characters.Length);
                return characters;
            } finally {
                Marshal.ZeroFreeBSTR(intPtr);
            }
        }

        /// <summary>
        /// Convert Char[] to base64 encoding.
        /// </summary>
        /// <param name="argCharacters">Char[] to convert.</param>
        /// <returns>Base 64 Encoding.</returns>
        [DebuggerStepThrough]
        public static String CreateBase64EncodedString(this Char[] argCharacters) {
            var byteArray = Encoding.UTF8.GetBytes(argCharacters);
            try {
                return byteArray.CreateBase64EncodedString();
            } finally {
                byteArray.ClearBytes();
            }
        }

        /// <summary>
        /// Convert Byte[] to base64 encoding.
        /// </summary>
        /// <param name="argBytes">Byte[] to convert.</param>
        /// <returns>Base 64 Encoding.</returns>
        [DebuggerStepThrough]
        public static String CreateBase64EncodedString(this Byte[] argBytes) {
            return Convert.ToBase64String(argBytes, 0, argBytes.Length);
        }

        /// <summary>
        /// Convert Byte[] to base64 encoded Char[].
        /// </summary>
        /// <param name="argBytes">Byte[] to convert.</param>
        /// <returns>Base 64 Encoded Char[].</returns>
        [DebuggerStepThrough]
        public static Char[] CreateBase64EncodedCharacters(this byte[] argBytes) {
            var tempCharacters = new char[argBytes.Length + argBytes.Length];
            try {
                var charArrayLength = Convert.ToBase64CharArray(argBytes, 0, argBytes.Length, tempCharacters, 0, Base64FormattingOptions.None);
                var characters = new Char[charArrayLength];
                Array.Copy(tempCharacters, characters, characters.Length);
                return characters;
            } finally {
                tempCharacters.ClearCharacters();
            }
        }

        /// <summary>
        /// Convert  String to Char[].
        /// </summary>
        /// <param name="argString">String to convert.</param>
        /// <returns>Char[]</returns>
        [DebuggerStepThrough]
        public static Char[] CreateCharacters(this String argString) {
            return argString.ToCharArray();
        }

        /// <summary>
        /// Convert base64 encoded String to Char[].
        /// </summary>
        /// <param name="argString">base64 encoded String to convert.</param>
        /// <returns>Char[]</returns>
        [DebuggerStepThrough]
        public static Char[] CreateCharactersFromBase64EncodedString(this String argString) {
            var bytes = Convert.FromBase64String(argString);
            try {
                return bytes.CreateCharacters();
            } finally {
                bytes.ClearBytes();
            }
        }

        /// <summary>
        /// Convert base64 encoded String to Byte[].
        /// </summary>
        /// <param name="argString">base64 encoded String to convert.</param>
        /// <returns>Byte[]</returns>
        [DebuggerStepThrough]
        public static Byte[] CreateBytesFromBase64EncodedString(this String argString) {
            return Convert.FromBase64String(argString);
        }

        /// <summary>
        /// Convert String to base64 encoding.
        /// </summary>
        /// <param name="argString">String to convert.</param>
        /// <returns>Base 64 Encoding.</returns>
        [DebuggerStepThrough]
        public static String CreateBase64EncodedString(this String argString) {
            var charArray = argString.ToCharArray();
            try {
                return charArray.CreateBase64EncodedString();
            } finally {
                charArray.ClearCharacters();
            }
        }

        /// <summary>
        /// Convert base64 encoded String to String.
        /// </summary>
        /// <param name="argString">base64 encoded String to convert.</param>
        /// <returns>String</returns>
        [DebuggerStepThrough]
        public static String CreateStringBase64String(this String argString) {
            var bytes = Convert.FromBase64String(argString);
            try {
                return Encoding.UTF8.GetString(bytes);
            } finally {
                bytes.ClearBytes();
            }
        }

        /// <summary>
        /// Copies '/0' to all elements of the array.
        /// </summary>
        /// <param name="argCharacters">Array to be over written with '/0'.</param>
        [DebuggerStepThrough]
        public static void ClearCharacters(this Char[] argCharacters) { Array.Clear(argCharacters, 0, argCharacters.Length); }

        /// <summary>
        /// Copies '/0' to all elements of the array.
        /// </summary>
        /// <param name="argBytes">Array to be over written with '/0'.</param>
        [DebuggerStepThrough]
        public static void ClearBytes(this Byte[] argBytes) { Array.Clear(argBytes, 0, argBytes.Length); }

        /// <summary>
        /// Convert Byte[] to Char[];
        /// </summary>
        /// <param name="argBytes">Byte[] to convert.</param>
        /// <returns>Char[]</returns>
        [DebuggerStepThrough]
        public static Char[] CreateCharacters(this Byte[] argBytes) {
            return Encoding.Unicode.GetChars(argBytes, 0, argBytes.Length);
        }

        /// <summary>
        /// Convert Char[] to Byte[];
        /// </summary>
        /// <param name="argCharacters">Char[] to convert.</param>
        /// <returns>Byte[]</returns>
        [DebuggerStepThrough]
        public static Byte[] CreateBytes(this Char[] argCharacters) {
            return Encoding.Unicode.GetBytes(argCharacters, 0, argCharacters.Length);
        }

        /// <summary>
        /// Convert String to Byte[];
        /// </summary>
        /// <param name="argString">String to convert.</param>
        /// <returns>Byte[]</returns>
        [DebuggerStepThrough]
        public static Byte[] CreateBytes(this String argString) {
            return Encoding.Unicode.GetBytes(argString);
        }

        /// <summary>
        /// Convert Char[] to Byte[];
        /// </summary>
        /// <param name="argCharacters">Char[] to convert.</param>
        /// <returns>Byte[]</returns>
        [DebuggerStepThrough]
        public static String CreateString(this Char[] argCharacters) {
            var bytes = argCharacters.CreateBytes();
            try {
                return Encoding.Unicode.GetString(bytes, 0, bytes.Length);
            } finally {
                bytes.ClearBytes();
            }
        }
    }
}
#endregion

