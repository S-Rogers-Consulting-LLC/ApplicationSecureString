# ApplicationSecureString
The 'ApplicationSecureString' is a direct replacement for the C# 'System.String'. The 'ApplicationSecureString' implements a wrapped 'SecureString' in order to eliminate or reduce the surface area of a Memory Scraping attack. ApplicationSecureString is a direct replacement for 'System.String' for all Results, Members, Properties, and as a Parameters. In addition the 'ApplicationSecureString' can be used in transport technologies like WCF, Remoting and ProtoBuf.

# Why ApplicationSecureString
Though data encryption is widely used to secure data, memory scraping finds weak areas from which it can take data. For example, some memory-scraping malware steals encrypted data from applications through which the data passed unencrypted.
