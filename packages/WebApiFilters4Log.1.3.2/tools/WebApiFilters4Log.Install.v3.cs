using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
	
public sealed class FileChangerV3
{
	public string Install(string path)
	{
		var files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories).Select(file => file.ToLower()).ToList();

		string startupFile = files.Where(file => file.Contains("startup.cs")).FirstOrDefault();

		if (string.IsNullOrWhiteSpace(startupFile)) return "ERROR - Startup.cs file not found!";

		if (!File.Exists(startupFile))
			return "ERROR - Startup.cs file not found!";

		var configured = ExistConfig(files);

		if (!configured.Item1 || !configured.Item2)
		{
			ChangeStartupFile(startupFile, configured.Item1, configured.Item2);

			string msg = !configured.Item1 ? "WebApiFilters4Log" : string.Empty;

			if (!configured.Item2) msg = string.Format("{0}log4Net", !configured.Item1 ? " and " : string.Empty);

			return string.Format("OK - {0} configured.", msg);
		}

		return "OK - Nothing to change.";
	}

	public Tuple<bool, bool> ExistConfig(List<string> files)
	{
		bool webApiFiltersConfig = false;
		bool log4NetConfig = false;
		
		files.AsParallel()
		.ForAll(file =>
		{
			string contentFile = File.ReadAllText(file);

			if (!webApiFiltersConfig) webApiFiltersConfig = contentFile.Contains("Action4LogFilter");
			if (!log4NetConfig) log4NetConfig = contentFile.Contains("log4net") && contentFile.Contains("XmlConfigurator.Configure");
		});

		return new Tuple<bool, bool>(webApiFiltersConfig, log4NetConfig);
	}

	private void ChangeStartupFile(string startupFile, bool addWebApiFilterConfig, bool addLog4NetConfig)
	{
		Console.WriteLine("Changing " + startupFile);
		var lines = File.ReadAllLines(startupFile);

		List<string> linesNewFile = new List<string>();

		bool methodNameFound = false;
		bool log4netConfiguraAdded = addLog4NetConfig;
		bool webApiFilterConfiguraAdded = addWebApiFilterConfig;
		foreach (var line in lines)
		{
			linesNewFile.Add(line);

			if (!methodNameFound && line.Contains("Configuration(IAppBuilder")) methodNameFound = true;

			if (methodNameFound && !log4netConfiguraAdded && line.Contains("{"))
			{
				linesNewFile.Add("\t\t\t// Configura o log4net (NÃO REMOVER)");
				linesNewFile.Add("\t\t\tlog4net.Config.XmlConfigurator.Configure();");
				log4netConfiguraAdded = true;
			}

			if (methodNameFound && log4netConfiguraAdded && line.Contains("HttpConfiguration") && !webApiFilterConfiguraAdded)
			{
				webApiFilterConfiguraAdded = true;

				string varName = string.Empty;

				if (line.Contains("new HttpConfiguration"))
				{
					if (line.Trim().StartsWith("HttpConfiguration"))
					{
						var tmp = line.Trim().Replace("HttpConfiguration ", string.Empty);
						varName = tmp.Substring(0, tmp.IndexOf("=")).Trim();
					}
					else if (line.Contains("var"))
					{
						var tmp = line.Trim().Replace("var ", string.Empty);
						varName = tmp.Substring(0, tmp.IndexOf("=")).Trim();
					}
					else
					{
						varName = line.Trim().Substring(0, line.Trim().IndexOf("=")).Trim();
					}
				}

				if (!string.IsNullOrWhiteSpace(varName))
				{
					linesNewFile.Add("\t\t\t// O inicio e o fim de todas acoes serao registradas como DEBUG no Logger.");
					linesNewFile.Add("\t\t\t// WARN caso a duracao ultrapasse 60 segundos e ERROR em caso de excecao.");
					linesNewFile.Add("\t\t\t// Remova a propriedade MonitoredTypes para desbilitar o log de todos os argumentos da requisicao.");
					linesNewFile.Add(string.Format("\t\t\t{0}.Filters.Add(new WebApiFilters4Log.Action4LogFilterAttribute(\"Logger\", WebApiFilters4Log.LogLevel.DEBUG, 60) {{ MonitoredTypes = \"*\" }});", varName));
					linesNewFile.Add("\t\t\t// Todas as excecoes nao tratadas serao registradas no ExceptionLogger e o resultado sera tratado e retornado adequadamente");
					linesNewFile.Add(string.Format("\t\t\t{0}.Filters.Add(new WebApiFilters4Log.Exception4LogFilterAttribute(\"ExceptionLogger\"));", varName));
				}
			}
		}

		File.WriteAllLines(startupFile, linesNewFile.ToArray());
	}
}