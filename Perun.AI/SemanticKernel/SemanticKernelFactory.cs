using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace Perun.AI.SemanticKernel;

public sealed class SemanticKernelFactory(
    IKernelBuilderFactory kernelBuilderFactory,
    IEnumerable<IKernelPlugin> plugins,
    ILogger<SemanticKernelFactory> logger)
    : ISemanticKernelFactory
{
    public Kernel CreateKernelWithPlugins()
    {
        var kernel = kernelBuilderFactory.CreateKernelBuilderWithResiliency().Build();

        if (plugins != null && plugins.Any())
        {
            foreach (var plugin in plugins)
            {
                kernel.Plugins.AddFromObject(plugin);
            }
        }
        else
        {
            logger.LogWarning("No kernel plugins registered.");
        }
       
        return kernel;
    }
    
   
}