    using Firebase.Storage;
    using Google.Apis.Auth.OAuth2;
    using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
    using Microsoft.AspNetCore.Hosting;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    namespace SanGiaoDich_BrotherHood.Server.Services
    {
        public class FirebaseStorageService
        {
            private readonly string firebaseStorageBucket = "dbbrotherhood-ac2f1.appspot.com";
            private readonly string jsonFilePath;
            private readonly IWebHostEnvironment _env;
            public FirebaseStorageService(IWebHostEnvironment env)
            {
                _env = env;
                jsonFilePath = Path.Combine(_env.WebRootPath, "js", "dbbrotherhood-ac2f1-firebase-adminsdk-mwsoo-4b867b2d83.json");
            }
            public async Task<string> UploadFileToFirebaseStorage(Stream fileStream, string fileName)
            {
                var credential = GoogleCredential.FromFile(jsonFilePath);
                var firebaseStorage = new FirebaseStorage(firebaseStorageBucket, new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = async () => (await credential.UnderlyingCredential.GetAccessTokenForRequestAsync(CancellationToken.None.ToString()))
                });
                var task = firebaseStorage.Child("ImageTest").Child(fileName).PutAsync(fileStream);
                var downloadUrl = await task;

                return downloadUrl;
            }
        }
    }
