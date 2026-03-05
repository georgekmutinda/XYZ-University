// This is where we generate the secret keys for JWT signing. In a production environment, you would want to store these securely, such as in environment variables or a secrets manager.
using System;
namespace XYZUniversityAPI.API
{
    public static class Keys
    {        
        // Just testing the pipeline
        public static string GenerateSecretKey()
        {
            // In a real application, you would want to use a more secure method of generating and storing keys.
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }
}
}