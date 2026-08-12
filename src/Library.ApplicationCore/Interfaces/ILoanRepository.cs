using Library.ApplicationCore.Entities;

namespace Library.ApplicationCore;

public interface ILoanRepository {
    Task<Loan?> GetLoan(int loanId);
    Task<List<Loan>> GetLoansForBookItems(IEnumerable<int> bookItemIds);
    Task UpdateLoan(Loan loan);
}