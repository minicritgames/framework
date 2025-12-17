using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Minikit
{
    public class MKJsonConverter_Sprite : MKJsonConverter
    {
        private const string fileKey = "file";
        private const string pivotKey = "pivot";
        
        
        public override void WriteJson(JsonWriter _writer, object _value, JsonSerializer _serializer)
        {
#if UNITY_EDITOR
            Sprite sprite = _value as Sprite;
            if (sprite == null)
            {
                _writer.WriteNull();
                return;
            }
            
            if (MKStreamingAssets.GetStreamingAssetsRelativePath(sprite, "Sprites/Public/", "Sprites/", false) is not string path)
            {
                _writer.WriteNull();
                return;
            }

            JObject jo = new()
            {
                [fileKey] = path,
                [pivotKey] = new JArray(sprite.pivot.x, sprite.pivot.y)
            };

            jo.WriteTo(_writer);
#else
            _writer.WriteNull();
            return;
#endif // UNITY_EDITOR
        }

        public override object ReadJson(JsonReader _reader, Type _objectType, object _existingValue, JsonSerializer _serializer)
        {
            if (_reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            JObject jo = JObject.Load(_reader);

            string fileName = jo[fileKey]?.ToString();
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            // Load metadata
            float pivotX = jo[pivotKey]?[0]?.Value<float>() ?? 0.5f;
            float pivotY = jo[pivotKey]?[1]?.Value<float>() ?? 0.5f;

            // Build full path
            string fullPath = Path.Combine(Application.streamingAssetsPath, fileName);

            if (!File.Exists(fullPath))
            {
                Debug.LogError($"Sprite .png not found at: {fullPath}");
                return null;
            }

            // Load PNG as Texture
            byte[] bytes = File.ReadAllBytes(fullPath);
            Texture2D tex = new(2, 2, TextureFormat.RGBA32, false);
            tex.LoadImage(bytes);

            // Create Sprite
            Sprite sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(pivotX, pivotY)
            );

            sprite.name = Path.GetFileNameWithoutExtension(fileName);

            return sprite;
        }

        public override bool CanConvert(Type _objectType)
        {
            return _objectType == typeof(Sprite);
        }
    }
} // Minikit namespace
