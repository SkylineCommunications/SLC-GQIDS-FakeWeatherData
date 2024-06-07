namespace GQIIntegrationSPI
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using Skyline.DataMiner.Analytics.GenericInterface;

	[GQIMetaData(Name = "Belgium weather api (aka TotallyUnpredictableWeatherApi) ")]
	public class GQIDataSourceFromCSVDouble : IGQIDataSource, IGQIInputArguments
    {
        private readonly GQIDateTimeArgument _argStart = new GQIDateTimeArgument("Start Time") { IsRequired = true };
        private readonly GQIDateTimeArgument _argEnd = new GQIDateTimeArgument("End Time") { IsRequired = true };
        private readonly GQIIntArgument _argIntervals = new GQIIntArgument("Number of intervals") { IsRequired = true };
        private readonly Random _random = new Random();

        private DateTime _start;
        private DateTime _end;
        private int _numberOfIntervals;

        public GQIArgument[] GetInputArguments()
        {
            return new GQIArgument[] { _argStart, _argEnd, _argIntervals };
        }

        public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
        {
            _start = args.GetArgumentValue(_argStart);
            _end = args.GetArgumentValue(_argEnd);
            _numberOfIntervals = args.GetArgumentValue(_argIntervals);

            return new OnArgumentsProcessedOutputArgs();
        }

        /// <inheritdoc />
        public GQIColumn[] GetColumns()
        {
            return new GQIColumn[]
            {
                new GQIStringColumn("Weather type"),
                new GQIDateTimeColumn("Start"),
                new GQIDateTimeColumn("End"),
            };
        }

        /// <inheritdoc />
        public GQIPage GetNextPage(GetNextPageInputArgs args)
        {
            List<GQIRow> rows = new List<GQIRow>();
            TimeSpan interval = TimeSpan.FromTicks((_end - _start).Ticks / _numberOfIntervals);
            DateTime start = _start;
            DateTime end = start + interval;

            for (int i = 0; i < _numberOfIntervals; i++)
            {
                rows.Add(new GQIRow(new[]
				{
                    new GQICell { Value = GetWeather() },
                    new GQICell { Value = start.ToUniversalTime()},
                    new GQICell { Value = end.ToUniversalTime()},
				}));

                start = end;
                end = start + interval;
            }

            return new GQIPage(rows.ToArray())
            {
                HasNextPage = false,
            };
        }

        private String GetWeather()
        {
            string[] options = { "rain", "sun" };

            // Generate a random index
            int index = _random.Next(0, options.Length);

            // Select the string based on the index
            return options[index];
        }
    }
}