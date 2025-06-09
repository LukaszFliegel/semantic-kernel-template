using Microsoft.SemanticKernel;

namespace Perun.AI.SemanticKernel;

public interface ISemanticKernelFactory
{
    Kernel CreateKernelWithPlugins();
}