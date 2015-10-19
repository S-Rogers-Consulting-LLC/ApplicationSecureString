# ApplicationSecureString
Direct Replacement for C# 'System.String' that implement a wrapped SecureString in order to eliminate or reduce the surface area of a Memory Scraping attack.

Though data encryption is widely used to secure data, memory scraping finds weak areas from which it can take data. For example, some memory-scraping malware steals encrypted data from applications through which the data passed unencrypted.
