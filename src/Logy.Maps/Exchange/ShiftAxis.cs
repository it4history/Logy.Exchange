using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Logy.Maps.ReliefMaps.Water;
using Logy.Maps.ReliefMaps.World.Ocean;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Logy.Maps.Exchange
{
    public class ShiftAxis : Algorythm<Basin3>
    {
        public ShiftAxis() {}

        public ShiftAxis(BasinData data) : base(data)
        {
        }

        [IgnoreDataMember]
        public PoleNorth DesiredPoleNorth { get; set; } = new PoleNorth { X = -40, Y = 73 };

        public bool Slow { get; set; }

        /// <summary>
        /// key - frame
        /// </summary>
        public Dictionary<int, PoleNorth> Poles { get; set; } = new Dictionary<int, PoleNorth>
        {
            {
                -1, new PoleNorth { X = 0, Y = 90 }
            }
        };

        public BasinData Data
        {
            get { return (BasinData)DataAbstract; }
            set { DataAbstract = value; }
        }
    }

    public class BundleConverter : JsonConverter
    {
        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public BundleConverter()
        { }
        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.Null)
            {
                if (reader.TokenType == JsonToken.StartObject)
                {
                    JToken token = JToken.Load(reader);
                    return token.ToObject(objectType);
                }
                else
                {
                    JValue jValue = new JValue(reader.Value);
                    switch (reader.TokenType)
                    {
                        case JsonToken.String:
                            break;
                        case JsonToken.Date:
                            break;
                        case JsonToken.Boolean:
                            break;
                        case JsonToken.Integer:
                            int i = (int)jValue;
                            break;
                        default:
                            break;
                    }
                }
            }
            return reader.Value;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }

}