using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

public class RedditPostsManager : MonoBehaviour
{
    public struct OAuthTokenInfo
    {
        public string access_token;
        public string token_type;
        public int expires_in;
        public string scope;
    }

    private bool isAuthorized = false;
    private OAuthTokenInfo _tokenInfo;
    public OAuthTokenInfo TokenInfo
    {
        get { return _tokenInfo; }
        private set
        {
            _tokenInfo = value;
            isAuthorized = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetOauthToken(OnAuthorized));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    string GetBasicAuth(string username, string password)
    {
        string auth = username + ":" + password;
        auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
        auth = "Basic " + auth;
        return auth;
    }

    IEnumerator GetOauthToken(System.Action callback)
    {
        WWWForm data = new WWWForm();
        data.AddField("grant_type", "password");
        data.AddField("username", "arumeria");
        data.AddField("password", "midori123=");

        var request = UnityWebRequest.Post("https://www.reddit.com/api/v1/access_token", data);
        string basicAuth = GetBasicAuth("t5Dp83149cK_BQ", "YRXwMlCvnEzq9oSb5ilbQciJmrkwZQ");
        request.SetRequestHeader("Authorization", basicAuth);
        request.SetRequestHeader("User-Agent", "Doomscroller3DScript/0.0.1");

        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debug.Log("Request failed");
        }
        else
        {
            Debug.Log($"Request succeeded, response {request.responseCode}");
            TokenInfo = JsonUtility.FromJson<OAuthTokenInfo>(request.downloadHandler.text);
            Debug.Log($"Access Token: {TokenInfo.access_token}");
            callback.Invoke();
        }
    }

    void OnAuthorized()
    {
        StartCoroutine(GetSubredditPosts("PS5", delegate(IList<PostInfo> posts) {
            foreach(PostInfo p in posts)
            {
                Debug.Log($"[{p.score}] {p.title}: {p.selftext} | {p.num_comments} comments");
            }
        }));
    }

    public struct PostInfo
    {
        public string title;
        public string thumbnail;
        public string selftext;
        public int num_comments;
        public int score;
    }

    IEnumerator GetSubredditPosts(string subreddit, System.Action<IList<PostInfo>> callback)
    {
        if (!isAuthorized)
        {
            yield return false;
        }

        var req = UnityWebRequest.Get($"https://oauth.reddit.com/r/{subreddit}/hot");
        req.SetRequestHeader("User-Agent", "Doomscroller3DScript/0.0.1");
        req.SetRequestHeader("Authorization", $"bearer {TokenInfo.access_token}");
        yield return req.SendWebRequest();

        if (req.isNetworkError)
        {
            Debug.Log($"Request failed, response code {req.responseCode}");
        }
        else
        {
            Debug.Log($"Request success, response code {req.responseCode}");
            JObject res = JObject.Parse(req.downloadHandler.text);
            JArray posts = (JArray)res["data"]["children"];
            Debug.Log($"Posts: {posts.ToString()}");

            IList<PostInfo> subPosts = posts.Select(p => new PostInfo
            {
                title = WebUtility.HtmlDecode((string)p["data"]["title"]),
                thumbnail = (string)p["data"]["thumbnail"],
                selftext = (string)p["data"]["selftext"],
                num_comments = (int)p["data"]["num_comments"],
                score = (int)p["data"]["score"]
            }).ToList();

            callback.Invoke(subPosts);
        }
    }

    // public class BlogPost
    // {
    //     public string Title { get; set; }
    //     public string AuthorName { get; set; }
    //     public string AuthorTwitter { get; set; }
    //     public string Body { get; set; }
    //     public System.DateTime PostedDate { get; set; }
    // }

    // void TestJson()
    // {
    //     string sampleJson = @"[
    //         {
    //             'Title': 'Json.NET is awesome!',
    //             'Author': {
    //             'Name': 'James Newton-King',
    //             'Twitter': '@JamesNK',
    //             'Picture': '/jamesnk.png'
    //             },
    //             'Date': '2013-01-23T19:30:00',
    //             'BodyHtml': '&lt;h3&gt;Title!&lt;/h3&gt;\r\n&lt;p&gt;Content!&lt;/p&gt;'
    //         }
    //     ]";
    //     JArray blogPostArray = JArray.Parse(sampleJson);

    //     IList<BlogPost> blogPosts = blogPostArray.Select(p => new BlogPost
    //     {
    //         Title = (string)p["Title"],
    //         AuthorName = (string)p["Author"]["Name"],
    //         AuthorTwitter = (string)p["Author"]["Twitter"],
    //         PostedDate = (System.DateTime)p["Date"],
    //         Body = WebUtility.HtmlDecode((string)p["BodyHtml"])
    //     }).ToList();

    //     Debug.Log(blogPosts[0].Body);
    // }
}
