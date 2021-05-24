using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
namespace cli_life.config {
	[Serializable]
	public class Config {
		#region Properties
		public int width { get; set; }
		public int height { get; set; }
		public int cellSize { get; set; }
		public double liveDensity { get; set; }
		public static Config DefaultConfig => new Config(50,20,1,0.5);
		#endregion
		#region Construct
		public Config(int width, int height, int cellSize, double liveDensity) {
			this.width = width;
			this.height = height;
			this.cellSize = cellSize;
			this.liveDensity = liveDensity;
		}
		public Config() {
			var c = DefaultConfig;
			width = c.width;
			height = c.height;
			cellSize = c.cellSize;
			liveDensity = c.liveDensity;
		}
		#endregion
		#region Public Methods
		public void SaveConfig(string path) {
			StreamWriter stream = new StreamWriter(path);
			string jsonString = JsonSerializer.Serialize(this);
			stream.Write(jsonString);
			stream.Close();
		}
		#endregion
		#region Static Methods
		public static Config LoadConfig(string path) {
			StreamReader stream = new StreamReader(path);
			Config c = JsonSerializer.Deserialize<Config>(stream.ReadToEnd());
			stream.Close();
			return c;
		}
		#endregion
	}
}
