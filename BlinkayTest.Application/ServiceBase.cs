using BlinKayTest.Shared;

namespace BlinkayTest.Application;

public abstract class ServiceBase<TService>
{
    protected IUnitOfWork _unitOfWork;


    protected ServiceBase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
}
