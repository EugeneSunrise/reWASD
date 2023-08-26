using System;
using System.Windows.Media;

namespace reWASDUI.ViewModels.CommunityConfigs
{
	public class Gamepad
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public string UIName
		{
			get
			{
				return this.Name + (this.IsLast ? "" : ", ");
			}
		}

		public bool IsLast { get; set; }

		public SolidColorBrush Brush
		{
			get
			{
				new ColorConverter();
				string name = this.Name;
				string text;
				if (name != null)
				{
					switch (name.Length)
					{
					case 3:
						if (!(name == "NES"))
						{
							goto IL_788;
						}
						break;
					case 4:
						if (!(name == "SNES"))
						{
							goto IL_788;
						}
						break;
					case 5:
					{
						char c = name[0];
						if (c != 'I')
						{
							if (c != 'S')
							{
								goto IL_788;
							}
							if (!(name == "Steam"))
							{
								goto IL_788;
							}
							text = "#5D72DF";
							goto IL_78E;
						}
						else
						{
							if (!(name == "Ipega"))
							{
								goto IL_788;
							}
							text = "#A52A2A";
							goto IL_78E;
						}
						break;
					}
					case 6:
					{
						char c = name[0];
						if (c <= 'A')
						{
							if (c != '8')
							{
								if (c != 'A')
								{
									goto IL_788;
								}
								if (!(name == "Azeron"))
								{
									goto IL_788;
								}
								text = "#2DA358";
								goto IL_78E;
							}
							else if (!(name == "8BitDo"))
							{
								goto IL_788;
							}
						}
						else if (c != 'N')
						{
							if (c != 'X')
							{
								goto IL_788;
							}
							if (name == "Xbox S")
							{
								text = "#3D7BE7";
								goto IL_78E;
							}
							if (!(name == "Xbox X"))
							{
								goto IL_788;
							}
							text = "#3D7BE7";
							goto IL_78E;
						}
						else
						{
							if (!(name == "NS Pro"))
							{
								goto IL_788;
							}
							text = "#FA8C8C";
							goto IL_78E;
						}
						break;
					}
					case 7:
					{
						char c = name[0];
						if (c != 'G')
						{
							if (c != 'J')
							{
								goto IL_788;
							}
							if (!(name == "Joy-Con"))
							{
								goto IL_788;
							}
							text = "#14BEDD";
							goto IL_78E;
						}
						else
						{
							if (!(name == "Gamepad"))
							{
								goto IL_788;
							}
							text = "#15ADFB";
							goto IL_78E;
						}
						break;
					}
					case 8:
					{
						char c = name[5];
						if (c != '3')
						{
							if (c != 'O')
							{
								if (c != 'u')
								{
									goto IL_788;
								}
								if (!(name == "GameCube"))
								{
									goto IL_788;
								}
								text = "#14B1A1";
								goto IL_78E;
							}
							else
							{
								if (!(name == "Xbox One"))
								{
									goto IL_788;
								}
								text = "#3D7BE7";
								goto IL_78E;
							}
						}
						else
						{
							if (!(name == "Xbox 360"))
							{
								goto IL_788;
							}
							text = "#24A135";
							goto IL_78E;
						}
						break;
					}
					case 9:
					{
						char c = name[0];
						if (c <= 'H')
						{
							if (c != 'D')
							{
								if (c != 'H')
								{
									goto IL_788;
								}
								if (!(name == "HORI Mini"))
								{
									goto IL_788;
								}
								text = "#1BBDD3";
								goto IL_78E;
							}
							else
							{
								if (!(name == "DualSense"))
								{
									goto IL_788;
								}
								text = "#8595DE";
								goto IL_78E;
							}
						}
						else if (c != 'W')
						{
							if (c != 'X')
							{
								goto IL_788;
							}
							if (!(name == "Xim Nexus"))
							{
								goto IL_788;
							}
							goto IL_768;
						}
						else
						{
							if (!(name == "Wii U Pro"))
							{
								goto IL_788;
							}
							goto IL_738;
						}
						break;
					}
					case 10:
					{
						char c = name[0];
						if (c <= 'S')
						{
							if (c != 'G')
							{
								if (c != 'S')
								{
									goto IL_788;
								}
								if (!(name == "Switch Pro"))
								{
									goto IL_788;
								}
								text = "#CF4647";
								goto IL_78E;
							}
							else
							{
								if (!(name == "GameSir G7"))
								{
									goto IL_788;
								}
								goto IL_768;
							}
						}
						else if (c != 'W')
						{
							if (c != 'X')
							{
								goto IL_788;
							}
							if (!(name == "Xbox Elite"))
							{
								goto IL_788;
							}
							text = "#DFA130";
							goto IL_78E;
						}
						else
						{
							if (!(name == "Wii Remote"))
							{
								goto IL_788;
							}
							goto IL_738;
						}
						break;
					}
					case 11:
					{
						char c = name[4];
						if (c <= 'N')
						{
							if (c != ' ')
							{
								if (c != 'C')
								{
									if (c != 'N')
									{
										goto IL_788;
									}
									if (!(name == "Wii Nunchuk"))
									{
										goto IL_788;
									}
									goto IL_738;
								}
								else
								{
									if (!(name == "Wii Classic"))
									{
										goto IL_788;
									}
									goto IL_738;
								}
							}
							else
							{
								if (!(name == "Xbox Elite2"))
								{
									goto IL_788;
								}
								text = "#FFC150";
								goto IL_78E;
							}
						}
						else if (c != 'S')
						{
							if (c != 'e')
							{
								if (c != 'o')
								{
									goto IL_788;
								}
								if (!(name == "Azeron Cyro"))
								{
									goto IL_788;
								}
								goto IL_740;
							}
							else
							{
								if (!(name == "Wolverine 2"))
								{
									goto IL_788;
								}
								goto IL_758;
							}
						}
						else
						{
							if (name == "DualShock 4")
							{
								text = "#926AD8";
								goto IL_78E;
							}
							if (!(name == "DualShock 3"))
							{
								goto IL_788;
							}
							text = "#DB6BA1";
							goto IL_78E;
						}
						break;
					}
					case 12:
					{
						char c = name[0];
						if (c != 'G')
						{
							if (c != 'S')
							{
								goto IL_788;
							}
							if (!(name == "Sega Genesis"))
							{
								goto IL_788;
							}
						}
						else
						{
							if (!(name == "Gaming Mouse"))
							{
								goto IL_788;
							}
							text = "#8E87F3";
							goto IL_78E;
						}
						break;
					}
					case 13:
					{
						char c = name[0];
						if (c != 'A')
						{
							if (c != 'N')
							{
								goto IL_788;
							}
							if (!(name == "NVIDIA SHIELD"))
							{
								goto IL_788;
							}
							text = "#73B22A";
							goto IL_78E;
						}
						else
						{
							if (!(name == "Azeron Cybork"))
							{
								goto IL_788;
							}
							goto IL_740;
						}
						break;
					}
					case 14:
					{
						char c = name[13];
						if (c <= '2')
						{
							if (c != '1')
							{
								if (c != '2')
								{
									goto IL_788;
								}
								if (!(name == "Flydigi Apex 2"))
								{
									goto IL_788;
								}
								text = "#B5A9FF";
								goto IL_78E;
							}
							else
							{
								if (!(name == "Flydigi Apex 1"))
								{
									goto IL_788;
								}
								text = "#B5A9FF";
								goto IL_78E;
							}
						}
						else if (c != 'e')
						{
							if (c != 'n')
							{
								goto IL_788;
							}
							if (!(name == "PS3 Navigation"))
							{
								goto IL_788;
							}
							text = "#B452BA";
							goto IL_78E;
						}
						else
						{
							if (!(name == "Standard Mouse"))
							{
								goto IL_788;
							}
							text = "#696DFF";
							goto IL_78E;
						}
						break;
					}
					case 15:
					{
						char c = name[0];
						if (c != 'F')
						{
							if (c != 'G')
							{
								if (c != 'W')
								{
									goto IL_788;
								}
								if (!(name == "Wii Classic Pro"))
								{
									goto IL_788;
								}
								goto IL_738;
							}
							else
							{
								if (!(name == "Generic Gamepad"))
								{
									goto IL_788;
								}
								goto IL_768;
							}
						}
						else
						{
							if (!(name == "Flydigi Vader 2"))
							{
								goto IL_788;
							}
							text = "#D0C9FF";
							goto IL_78E;
						}
						break;
					}
					case 16:
						if (!(name == "RazerRaijuMobile"))
						{
							goto IL_788;
						}
						goto IL_758;
					case 17:
					{
						char c = name[1];
						if (c != 't')
						{
							if (c != 'w')
							{
								goto IL_788;
							}
							if (!(name == "Switch Online N64"))
							{
								goto IL_788;
							}
						}
						else
						{
							if (!(name == "Standard Keyboard"))
							{
								goto IL_788;
							}
							text = "#F64A23";
							goto IL_78E;
						}
						break;
					}
					case 18:
					case 20:
					case 21:
					case 23:
					case 24:
					case 25:
						goto IL_788;
					case 19:
						if (!(name == "Flydigi Vader 2 Pro"))
						{
							goto IL_788;
						}
						text = "#D0C9FF";
						goto IL_78E;
					case 22:
						if (!(name == "PowerA MOGA XP5-X Plus"))
						{
							goto IL_788;
						}
						text = "#B8860B";
						goto IL_78E;
					case 26:
						if (!(name == "Thrustmaster Dual Analog 4"))
						{
							goto IL_788;
						}
						text = "#A34242";
						goto IL_78E;
					default:
						goto IL_788;
					}
					text = "#7D53DE";
					goto IL_78E;
					IL_738:
					text = "#14F5F1";
					goto IL_78E;
					IL_740:
					text = "#61E491";
					goto IL_78E;
					IL_758:
					text = "#FF8201";
					goto IL_78E;
					IL_768:
					text = "#FFCF37";
					goto IL_78E;
				}
				IL_788:
				text = "#FFC150";
				IL_78E:
				return new SolidColorBrush((Color)ColorConverter.ConvertFromString(text));
			}
		}
	}
}
