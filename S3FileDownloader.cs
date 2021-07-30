/************************************************
 * S3FileDownloader
 * S3 File download module with AWS cognito
 * 2019/04/23
 * *********************************************/
using UnityEngine;
using Amazon.S3;
using Amazon.Runtime;
using System.IO;
using Amazon.CognitoIdentity;
using Amazon;
using Amazon.S3.Model;
using Amazon.S3.Util;

namespace AWSSDK.Examples
{
    public class S3FileDownloader : MonoBehaviour
    {
        #region public methods & variables

        private RegionEndpoint _CognitoIdentityRegion
        {
            get { return RegionEndpoint.GetBySystemName(Utils.CognitoIdentityRegion); }
        }

        //returns s3 region end point
        private RegionEndpoint _S3Region
        {
            get { return RegionEndpoint.GetBySystemName(Utils.S3Region); }
        }
        //Initialize the settings ...
        public void InitS3FileDownloader()
        {
            UnityInitializer.AttachToGameObject(this.gameObject);

            AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;

        }

        void Start()
        {
             //UnityInitializer.AttachToGameObject(this.gameObject);

             //AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
        }

        public static S3FileDownloader instance;
        public static S3FileDownloader Instance()
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(S3FileDownloader)) as S3FileDownloader;
            }
            return instance;
        }

        public void DownloadNSaveFile(string strObjId)
        {
            Client.GetObjectAsync(Utils.S3BucketName, strObjId, (responseObj) =>
            {
                var response = responseObj.Response;
                if (response == null)
                    Debug.Log("Response is Null");
                if (response.ResponseStream != null)
                {
                    using (var fs = File.Create(Path.Combine(Application.persistentDataPath + "/Raw/" ,strObjId+ Utils.GLTF_EXT)))
                    //using (var fs = File.Create(Utils.TEST_PATH + Utils.GLTF_FOLDER + strObjId + Utils.GLTF_EXT))
                    {
                        byte[] buffer = new byte[81920];
                        int count;
                        while ((count = response.ResponseStream.Read(buffer, 0, buffer.Length)) != 0)
                            fs.Write(buffer, 0, count);
                        fs.Flush();
                    }
                    EventManager.DoDownloadSuccess();
                }
                else
                {
                    EventManager.DoDownloadFail();
                }
            });
        }
        #endregion
        #region private methods & variables
        private IAmazonS3 _s3Client;
        private AWSCredentials _credentials;

        /*********************************************
         * Credentials:
         * create or return current credentials
         * from poolId and identityRegion Info
         * *******************************************/
        private AWSCredentials Credentials
        {
            get
            {
                if (_credentials == null)
                    _credentials = new CognitoAWSCredentials(Utils.IdentityPoolId, _CognitoIdentityRegion);
                return _credentials;
            }
        }

        private IAmazonS3 Client
        {
            get
            {
                if (_s3Client == null)
                {
                    _s3Client = new AmazonS3Client(Credentials, _S3Region);
                }
                return _s3Client;
            }
        }
        #endregion

    }
}
