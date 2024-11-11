using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using NativeGalleryNamespace; // Namespace may vary based on the plugin
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;

public class ImageUploader : MonoBehaviour
{
    public Button uploadButton;
    public Image displayImage; // Optional: To display the selected image
    public string aiResponse;

    public void OnUploadButtonClicked()
    {
        PickImage();
    }

    private void PickImage()
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                // Load the image into a Texture2D
                Texture2D texture = NativeGallery.LoadImageAtPath(path, 512, false);
                if (texture != null)
                {
                    // Optionally display the image
                    displayImage.sprite = SpriteFromTexture2D(texture);

                    // Proceed to upload
                    StartCoroutine(UploadImage(texture));
                }
                else
                {
                    Debug.Log("Couldn't load texture from " + path);
                }
            }
        }, "Select an image", "image/*");

        Debug.Log("Permission result: " + permission);
    }

    private Sprite SpriteFromTexture2D(Texture2D texture)
    {
        return Sprite.Create(texture,
                             new Rect(0, 0, texture.width, texture.height),
                             new Vector2(0.5f, 0.5f));
    }

    private IEnumerator UploadImage(Texture2D texture)
    {
        // Convert Texture2D to PNG or JPEG
        byte[] imageBytes = texture.EncodeToJPG(); // or EncodeToJPG()

        // Encode to Base64
        string base64Image = $"data:image/jpeg;base64,{System.Convert.ToBase64String(imageBytes)}";
       

        ApiRequestPayload payload = new ApiRequestPayload
        {
            model = "gpt-4o-mini",
            messages = new Message[]
       {
            new Message
            {
                role = "user",
                content = new ContentItem[]
                {
                    new ContentItem
                    {
                        Type = "text",
                        Text = "What bird species is this?"
                    },
                    new ContentItem
                    {
                        Type = "image_url",
                        ImageUrl = new ImageUrl
                        {
                            url = base64Image
                        }
                    }
                }
            }
       },
            max_tokens = 300
        };


        // Create JSON payload
        string jsonPayload = JsonConvert.SerializeObject(payload, Formatting.None);

        // Prepare the request
        string apiUrl = "https://api.openai.com/v1/chat/completions"; // Replace with your API URL
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + "YOURAPIKEYHERE"); // Handle API Key securely

        //// Send the request
        yield return request.SendWebRequest();

        // Handle the response
        if (request.result == UnityWebRequest.Result.Success)
        {
            ParseResponse(request.downloadHandler.text, displayImage.sprite);
            // Process the response as needed

        }
        else
        {
            Debug.LogError("Upload failed: " + request.error);
            Debug.LogError("SendApiRequest: API Request failed with status code " + request.responseCode + ". Error: " + request.error);
            Debug.LogError("SendApiRequest: Response Body: " + request.downloadHandler.text);
        }
    }

    private void ParseResponse(string jsonResponse, Sprite image)
    {
        try
        {
            ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);

            if (apiResponse != null && apiResponse.Choices != null && apiResponse.Choices.Count > 0)
            {
                string content = apiResponse.Choices[0].Message.Content;

                if (content == null)
                {
                    aiResponse = "No AI Response";
                }
                else
                {
                    aiResponse = content;
                }
                // TODO: Move under response
                var userGalleryBird = new UserBirdUploadData()
                {
                    userImage = displayImage.sprite,
                    aiDescription = aiResponse
                };
                PersistentDataManager.Instance.AddUserGalleryBird(userGalleryBird);

            }
            else
            {
                Debug.LogWarning("ParseResponse: No choices found in the response.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("ParseResponse: Exception during JSON deserialization: " + ex.Message);
        }
    }
}