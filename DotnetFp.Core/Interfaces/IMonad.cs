namespace DotnetFp.Core.Interfaces;

// Descriptive IMonad interfaces
public interface IMonad: IFunctor { }
public interface IMonad<T>: IFunctor<T>, IMonad { }