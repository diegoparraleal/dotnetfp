namespace DotnetFp.Core.Interfaces;

// Descriptive IMonad interfaces
public interface IMonad: IFunctor { }
public interface IMonad<T>: IFunctor<T>, IMonad { }
public interface IMonad<T1, T2>: IFunctor<T1, T2>, IMonad { }