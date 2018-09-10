Namespace AKP.Security.Encryption
    Module AES

        Public Function AES_Encrypt(ByVal Input As String, ByVal Pass As String) As String
            Dim AES As New System.Security.Cryptography.RijndaelManaged
            Dim Hash_AES As New System.Security.Cryptography.MD5CryptoServiceProvider
            Dim Encrypted As String = ""
            Try
                Dim Hash(31) As Byte
                Dim Temp As Byte() = Hash_AES.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(Pass))
                Array.Copy(Temp, 0, Hash, 0, 16)
                Array.Copy(Temp, 0, Hash, 15, 16)
                AES.Key = Hash
                AES.Mode = System.Security.Cryptography.CipherMode.ECB
                Dim DESEncrypter As System.Security.Cryptography.ICryptoTransform = AES.CreateEncryptor
                Dim Buffer As Byte() = System.Text.ASCIIEncoding.ASCII.GetBytes(Input)
                Encrypted = Convert.ToBase64String(DESEncrypter.TransformFinalBlock(Buffer, 0, Buffer.Length))
            Catch Ex As Exception
            End Try
            Return Encrypted
        End Function

        Public Function AES_Decrypt(ByVal Input As String, ByVal Pass As String) As String
            Dim AES As New System.Security.Cryptography.RijndaelManaged
            Dim Hash_AES As New System.Security.Cryptography.MD5CryptoServiceProvider
            Dim Decrypted As String = ""
            Try
                Dim Hash(31) As Byte
                Dim Temp As Byte() = Hash_AES.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(Pass))
                Array.Copy(Temp, 0, Hash, 0, 16)
                Array.Copy(Temp, 0, Hash, 15, 16)
                AES.Key = Hash
                AES.Mode = System.Security.Cryptography.CipherMode.ECB
                Dim DESDecrypter As System.Security.Cryptography.ICryptoTransform = AES.CreateDecryptor
                Dim Buffer As Byte() = Convert.FromBase64String(Input)
                Decrypted = System.Text.ASCIIEncoding.ASCII.GetString(DESDecrypter.TransformFinalBlock(Buffer, 0, Buffer.Length))
            Catch Ex As Exception
            End Try
            Return Decrypted
        End Function

    End Module
End Namespace