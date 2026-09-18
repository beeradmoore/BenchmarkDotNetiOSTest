using System.Threading.Tasks;
using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;

namespace BenchmarkDotNetiOSTest;

public class MainViewController : UIViewController
{
    private UITextView _textView;
    UIActivityIndicatorView _activityIndicatorView;
    
    public MainViewController()
    {
        _textView = new UITextView()
        {
            Editable = false,
            AutoresizingMask = UIViewAutoresizing.All,
        };
        View.AddSubview(_textView);

        _activityIndicatorView = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Large)
        {
            HidesWhenStopped = true,
        };
        View.AddSubview(_activityIndicatorView);
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
        
        _textView.Frame = View.Bounds;
       // _activityIndicatorView.Frame = View.Bounds;

        Task.Run(RunBenchmark);
    }
    
    void RunBenchmark()
    {
        UIDevice.CurrentDevice.BeginInvokeOnMainThread(() =>
        {
          //  _activityIndicatorView.StartAnimating();
            _textView.Text = "Running benchmark...";
        });
        
        try
        {
            var artifactsPath = Path.Combine(Path.GetTempPath(), "BenchmarkDotNetiOSTest", "Artifacts");
            if (Directory.Exists(artifactsPath) == false)
            {
                Directory.CreateDirectory(artifactsPath);
            }

            var logger = new AccumulationLogger();
            
            #if DEBUG
            var config = new DebugInProcessConfig()
                .AddJob(Job.Default.WithToolchain(new InProcessNoEmitToolchain(TimeSpan.FromMinutes(10), logOutput: true)))
                .AddDiagnoser(MemoryDiagnoser.Default)
                .WithArtifactsPath(artifactsPath);
            #else
            var config = ManualConfig.CreateMinimumViable()
                .AddJob(Job.Default.WithToolchain(new InProcessNoEmitToolchain(TimeSpan.FromMinutes(10), logOutput: true)))
                .AddDiagnoser(MemoryDiagnoser.Default)
                .WithArtifactsPath(artifactsPath);
            #endif
            
            config.UnionRule = ConfigUnionRule.AlwaysUseGlobal; // Overriding the default

            //config.AddLogger(logger);
            
            var summary = BenchmarkRunner.Run<BenchmarkTests>(config);

            MarkdownExporter.Console.ExportToLog(summary, logger);
            ConclusionHelper.Print(logger,
                summary.BenchmarksCases
                    .SelectMany(benchmark => benchmark.Config.GetCompositeAnalyser().Analyse(summary))
                    .Distinct()
                    .ToList());

            UIDevice.CurrentDevice.BeginInvokeOnMainThread(() =>
            {
               // _activityIndicatorView.StopAnimating();
                _textView.Text = logger.GetLog();
            });
        }
        catch (Exception err)
        {
            UIDevice.CurrentDevice.BeginInvokeOnMainThread(() =>
            {
                _activityIndicatorView.StopAnimating();
                _textView.Text = $"Error: {err.Message}";
            });
        }
    }
}