# Chipper

[![forthebadge](https://forthebadge.com/images/badges/made-with-c-sharp.svg)](https://forthebadge.com)
[![forthebadge](https://forthebadge.com/images/badges/contains-tasty-spaghetti-code.svg)](https://forthebadge.com)
[![forthebadge](https://forthebadge.com/images/badges/0-percent-optimized.svg)](https://forthebadge.com)

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/Q5Q0M8XY)

This project is a simple chipper proof of concept. It uses a symmetric key for encrypting and decrypt binary data.

Keys are generated based on a user-provided passphrase and stored locally in the user's application data (AppData) directory.

## Table of Contents <!-- omit in toc -->

- [Capabilities](#capabilities)
  - [Generate keys](#generate-keys)
  - [Encrypt data](#encrypt-data)
  - [Decrypt data](#decrypt-data)
- [Algorithm](#algorithm)
  - [Basic concept](#basic-concept)
  - [Code snippet](#code-snippet)
- [ChipperUI](#chipperui)


## Capabilities

### Generate keys

On Windows, keys are stored in the `%AppData%/zsotroav/chipper/` directory with a `.key.bin` extension.

Generate keys based on a passphrase. Keys generated from the same passphrase will always result in the same key file being generated.

### Encrypt data

Encrypt user-inputted data or binary data read from files. During encryption, the user can choose which method to use and which key to use for the encryption. The user may save the encrypted data to a `.enc.bin` file.

### Decrypt data

Decrypt encrypted data using a selected key. As with encryption, the input may come from a file or straight from the console. The decrypted data may also be saved to a file.

## Algorithm

The algorithm is stored in the `program.cs` without any optimizations as this code was only made to be a proof of concept and will not be optimized any time soon.

### Basic concept

The basic concept of the algorithm is to take the binary data of the input and the key and XOR them together. The key's data is repeated if necessary.

### Code snippet
_from [Program.cs line 185](https://github.com/zsotroav/chipper/blob/master/chiper/Program.cs#L185)_
```cs
private static byte[] EncryptData(byte[] InData, byte[] KeyData)
        {
            var EncData = new byte[InData.Length];
            int x = 0;
            for (int i = 0; i < InData.Length; i++)
            {
                if (KeyData.Length < i - x + 1)
                {
                    x += KeyData.Length;
                }
                EncData[i] = (byte)(KeyData[i - x] ^ InData[i]);
            }
            return EncData;
        }
```

## ChipperUI

An updated version of Chipper is available in the [zsotroav/ChipperUI](https://github.com/zsotroav/ChipperUI) repository.