# **ENCRYPTED FILE SYSTEM**

- When an operating system is running on a system without file encryption, access to files normally goes through OS-controlled user authentication and access control lists. However, if an attacker gains physical access to the computer, this barrier can be easily circumvented. One way, for example, would be to remove the disk and put it in another computer with an OS installed that can read the filesystem; another, would be to simply reboot the computer from a boot CD containing an OS that is suitable for accessing the local filesystem.

- The most widely accepted solution to this is to store the files encrypted on the physical media
---
- This implementation of Encrypted File System provides interactive console for the manipulation of files and folders present in the file system.

- It enables files to be uploaded from the base file system, downloaded to it, it provides API for creating and editing .txt files as well as sharing with other users whose account information are securely stored in the database using modern hashing functions.
---
- System provides 3 different ciphers as well as 3 hashing functions from the SHA family all implemented by calling OpenSSL functions from command line hence requiring valid vesrion of OpenSSL installed on the machine and placed in path.
- OpenSSL comes preinstalled with Git Bash and the version used during development is 1.1.1.
