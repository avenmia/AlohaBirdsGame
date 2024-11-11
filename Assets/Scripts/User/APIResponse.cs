using Newtonsoft.Json;
using System.Collections.Generic;
using System;

[Serializable]
public class ApiResponse
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("object")]
    public string Object { get; set; }

    [JsonProperty("created")]
    public long Created { get; set; }

    [JsonProperty("model")]
    public string Model { get; set; }

    [JsonProperty("choices")]
    public List<Choice> Choices { get; set; }

    [JsonProperty("usage")]
    public Usage Usage { get; set; }

    [JsonProperty("system_fingerprint")]
    public string SystemFingerprint { get; set; }
}

[Serializable]
public class Choice
{
    [JsonProperty("index")]
    public int Index { get; set; }

    [JsonProperty("message")]
    public ResponseMessage Message { get; set; }

    [JsonProperty("logprobs")]
    public object Logprobs { get; set; } // Use appropriate type if known

    [JsonProperty("finish_reason")]
    public string FinishReason { get; set; }
}

[Serializable]
public class ResponseMessage
{
    [JsonProperty("role")]
    public string Role { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }

    [JsonProperty("refusal")]
    public object Refusal { get; set; } // Use appropriate type if known
}

[Serializable]
public class Usage
{
    [JsonProperty("prompt_tokens")]
    public int PromptTokens { get; set; }

    [JsonProperty("completion_tokens")]
    public int CompletionTokens { get; set; }

    [JsonProperty("total_tokens")]
    public int TotalTokens { get; set; }

    [JsonProperty("prompt_tokens_details")]
    public TokensDetails PromptTokensDetails { get; set; }

    [JsonProperty("completion_tokens_details")]
    public TokensDetails CompletionTokensDetails { get; set; }
}

[Serializable]
public class TokensDetails
{
    [JsonProperty("cached_tokens")]
    public int CachedTokens { get; set; }

    [JsonProperty("audio_tokens")]
    public int AudioTokens { get; set; }

    [JsonProperty("reasoning_tokens")]
    public int ReasoningTokens { get; set; }

    [JsonProperty("accepted_prediction_tokens")]
    public int AcceptedPredictionTokens { get; set; }

    [JsonProperty("rejected_prediction_tokens")]
    public int RejectedPredictionTokens { get; set; }
}