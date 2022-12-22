using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Variant10
{
	abstract class Software
	{
		static public string[] SWdate = new string[3] {"Свободное", "Условно-бесплатное", "Коммерческое"};
		static public void GetAllSWInfo(Software[] sw)
		{
			for(int i = 0; i < sw.Length; i++)
			{
				sw[i].GetSoftwareInfo();
			}
		}
		abstract public void GetSoftwareInfo();
		abstract public bool MathingTerm();
	}

	class Free : Software
	{
		string name;
		string manufacturer;

		public Free(string name, string manufacturer)
		{
			this.name = name;
			this.manufacturer = manufacturer;
		}

		public override void GetSoftwareInfo()
		{
			Console.WriteLine($"Название ПО: {name}\n Производитель: {manufacturer}\n\n");
		}
		public override bool MathingTerm()
		{
			return true;
		}
	}

	class Shareware : Software
	{
		string name;
		string manufacturer;
		DateTime dateOfInstall;
		int termOfFreeUse;

		public Shareware(string name, string manufacturer, DateTime dateOfInstall, int termOfFreeUse)
		{
			this.name = name;
			this.manufacturer = manufacturer;
			this.dateOfInstall = dateOfInstall;
			this.termOfFreeUse = termOfFreeUse;
		}

		public override void GetSoftwareInfo()
		{
			Console.WriteLine("Название ПО: {0}\nПроизводитель: {1}\nДата установки: {2}\nСрок использования: {3}\n\n", name, manufacturer, dateOfInstall, termOfFreeUse);
		}
		public override bool MathingTerm()
		{
			if (DateTime.Now <= dateOfInstall.AddDays(termOfFreeUse)) return true;
			else return false;
		}
	}
	class Commercial : Software
	{
		string name;
		string manufacturer;
		float price;
		DateTime dateOfInstall;
		int termOfUse;

		public Commercial(string name, string manufacturer, float price, DateTime dateOfInstall, int termOfUse)
		{
			this.name = name;
			this.manufacturer = manufacturer;
			this.price = price;
			this.dateOfInstall = dateOfInstall;
			this.termOfUse = termOfUse;
		}

		public override void GetSoftwareInfo()
		{
			Console.WriteLine("Название ПО: {0}\nПроизводитель: {1}\nЦена: {2}\nДата установки: {3}\nСрок использования: {4}\n\n", name, manufacturer, price, dateOfInstall, termOfUse);
		}

		public override bool MathingTerm()
		{
			if (DateTime.Now <= dateOfInstall.AddDays(termOfUse)) return true;
			else return false;
		}
	}
	class Program
	{
		static string[] ReadFile(FileStream fileStream)
		{
			byte[] buf = new byte[fileStream.Length];
			fileStream.Read(buf, 0, buf.Length);
			string textFromFile = Encoding.Default.GetString(buf);
			string[] textSplit = textFromFile.Split('\n');

			for(int i = 0; i < textSplit.Length; i++) textSplit[i] = textSplit[i].Trim();
			return textSplit;
		}

		static Software[] SetSWFromFile()
		{
			Software[]? sw = null;
			FileStream filesw;
			string filePath;
			Console.WriteLine("Введите путь к файлу через \"C:\\...\"");
			while (true)
			{
				filePath = Console.ReadLine();
				filesw = new FileStream(filePath, FileMode.Open, FileAccess.Read);

				if (filesw.CanRead) break;
				Console.WriteLine("Введен некорректный путь.");
			}

			string[] textSplit = ReadFile(filesw);
			filesw.Close();

			int countsw = 0;

			for(int i = 0; i < textSplit.Length; i++)
			{
				if (textSplit[i] == Software.SWdate[0] || textSplit[i] == Software.SWdate[1] || textSplit[i] == Software.SWdate[2]) countsw++;
			}

			if(countsw == 0) throw new Exception("Ошибка. Вероятно файл заполнен неверно или файл пустой.");

			sw = new Software[countsw];
			string[] countstr = new string[5];

			int ifw = 0;
			int typeindex;

			for(int i = 0; i < sw.Length; i++)
			{
				typeindex = ifw;
				for(int j = ifw + 1; j < textSplit.Length; j++)
				{
					if (textSplit[j] == Software.SWdate[0] || textSplit[j] == Software.SWdate[1] || textSplit[j] == Software.SWdate[2])
					{
						ifw = j;
						break;
					}
					for(int l = 0; l < countstr.Length; l++)
					{
						if (l == 2 && textSplit[ifw] == Software.SWdate[0]) break;
						if (l == 4 && textSplit[ifw] == Software.SWdate[1]) break;
						countstr[l] = textSplit[j];
						j++;
					}
					j--;
				}
				if (textSplit[typeindex] == "Свободное") sw[i] = new Free(countstr[0], countstr[1]);
				else if (textSplit[typeindex] == "Условно-бесплатное") sw[i] = new Shareware(countstr[0], countstr[1], Convert.ToDateTime(countstr[2]), Convert.ToInt32(countstr[3]));
				else if (textSplit[typeindex] == "Коммерческое") sw[i] = new Commercial(countstr[0], countstr[1], Convert.ToSingle(countstr[2]), Convert.ToDateTime(countstr[3]), Convert.ToInt32(countstr[4]));
				else Console.WriteLine("Не получилось добавить ПО по индексу.");
			}
			Console.WriteLine("Данные о ПО записаны");
			return sw;
		}

		static void Main()
		{
			try
			{
				Software[] software = SetSWFromFile();
				int k;

				while(true)
				{
					Console.WriteLine("1 - Записать/перезаписать данные о ПО\n" +
									  "2 - Вывести все ПО\n" +
									  "3 - Поиск товаров по тому, истек ли у него срок использования\n" +
									  "0 - Выход из программы");
					while (true)
					{
						k = Convert.ToInt32(Console.ReadLine());
						if (k >= 0 || k <= 3) break;
						Console.WriteLine("Введите корректное значение");
					}
					if (k == 0) break;
					else if (k == 1)
					{
						Console.WriteLine("Действительно перезаписать данные?");
						string i;

						while (true)
						{
							i = Console.ReadLine().ToLower();
							if (i == "да" || i == "нет") break;
							Console.WriteLine("Введите корректные данные");
						}
						if (i == "нет") break;
						else software = SetSWFromFile();
					}
					else if (k == 2)
					{
						if (software == null)
						{
							Console.WriteLine("Данные из файла не записаны");
							break;
						}
						Console.WriteLine("Вывод всех ПО");
						Software.GetAllSWInfo(software);
					}
					else if (k == 3)
					{
						if (software.Length == 0) Console.WriteLine("Ошибка");
						for (int i = 0; i < software.Length; i++)
						{
							if (software[i].MathingTerm()) software[i].GetSoftwareInfo();
						}

					}
					else Console.WriteLine("Введите корректное значение\n");
				}
			}
			catch(FormatException)
			{
				Console.WriteLine("Ошибка. Введены неверные значения");
			}
			catch
			{
				Console.WriteLine("Ошибка. Вероятно, введены неверные параметры");
			}
		}
	}
}