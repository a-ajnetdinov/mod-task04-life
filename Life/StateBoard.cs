using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;

namespace cli_life.config {
	[Serializable]
	public class StateBoard {
		public List<string> columns { get; set; } = new List<string>();
		public Config config { get; set; }
		public StateBoard() {

		}
		public StateBoard(Board board) {
			for(int x = 0; x < board.Rows; x++) {
				string str = "";
				for(int y = 0; y < board.Columns; y++) {
					str += board.Cells[y,x].IsAlive ? '*' : ' ';
				}
				columns.Add(str);
			}
			config = board.config;
		}

		public static void SaveBoard(Board board, string path) {
			var sb = new StateBoard(board);
			StreamWriter stream = new StreamWriter(path+"-cfg.txt");
			stream.Write(JsonSerializer.Serialize(sb.config));
			stream.Close();
			stream = new StreamWriter(path + "-board.txt");
			foreach(var i in sb.columns) {
				stream.WriteLine(i);
			}
			stream.Close();
		}
		public static StateBoard LoadBoard(string path) {
			Config cfg = Config.LoadConfig(path + "-cfg.txt");
			StateBoard sb = new StateBoard();
			sb.config = cfg;
			StreamReader stream = new StreamReader(path + "-board.txt");
			string str = stream.ReadToEnd();
			var strs = str.Split("\n");
			foreach(var i in strs) {
				if(i.Length > 1)
					sb.columns.Add(i);
			}
			return sb;
		}
		public Cell[,] GetCell() {
			int _width = config.width / config.cellSize;
			int _height = config.height / config.cellSize;
			Cell[,] cell = new Cell[_width, _height];
			for(int x = 0; x < _width; x++) {
				for(int y = 0; y < _height; y++) {
					cell[x,y] = new Cell();
					cell[x,y].IsAlive = columns[y][x] == '*' ? true : false;
				}
			}
			return cell;
		}
	}
}
