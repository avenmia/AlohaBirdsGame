using Newtonsoft.Json;

[System.Serializable]
public class ContentItem
{
    [JsonProperty("type")]
    public string Type { get; set; }

    // Optional field: only used if Type == "text"
    [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
    public string Text { get; set; }

    // Optional field: only used if Type == "image_url"
    [JsonProperty("image_url", NullValueHandling = NullValueHandling.Ignore)]
    public ImageUrl ImageUrl { get; set; }
}