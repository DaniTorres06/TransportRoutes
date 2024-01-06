using Microsoft.Data.SqlClient;
using Polly;
using Polly.Retry;

namespace TransportRoutesData.Helpers
{
    public static class RetryHelper
    {
        public static readonly int[] SqlServerTransientErrors = new int[] { 926, 4060, 40197, 40501, 40613, 49918, 49919, 49920, 4221, 615 };

        public static bool IsSqlTransientError(SqlException ex)
        {
            return SqlServerTransientErrors.Contains(ex.Number);
        }

        public static SqlDataReader? ExecuteReaderWithRetry(this SqlCommand command)
        {
            try
            {
                RetryPolicy policy = Policy.HandleInner<SqlException>(IsSqlTransientError)
                        .WaitAndRetry(3, retry => TimeSpan.FromMilliseconds(retry * 100));

                SqlDataReader reader = policy.Execute(() => command.ExecuteReader());
                return reader;
            }
            catch (SqlException)
            {
                return null;
            }
        }

        public static int? ExecuteNonQueryWithRetry(this SqlCommand command)
        {
            try
            {
                RetryPolicy policy = Policy.HandleInner<SqlException>(IsSqlTransientError)
                        .WaitAndRetry(3, retry => TimeSpan.FromMilliseconds(retry * 100));

                int reader = policy.Execute(() => command.ExecuteNonQuery());
                return reader;
            }
            catch (SqlException)
            {
                return null;
            }
        }

        public static async Task<SqlDataReader?> ExecuteReaderWithRetryAsync(this SqlCommand command)
        {
            try
            {
                RetryPolicy policy = Policy.HandleInner<SqlException>(IsSqlTransientError)
                        .WaitAndRetry(3, retry => TimeSpan.FromMilliseconds(retry * 100));

                SqlDataReader reader = await policy.Execute(async () => await command.ExecuteReaderAsync());
                return reader;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<int?> ExecuteNonQueryWithRetryAsync(this SqlCommand command)
        {
            try
            {
                RetryPolicy policy = Policy.HandleInner<SqlException>(IsSqlTransientError)
                        .WaitAndRetry(3, retry => TimeSpan.FromMilliseconds(retry * 100));

                int reader = await policy.Execute(async () => await command.ExecuteNonQueryAsync());
                return reader;
            }
            catch (SqlException)
            {
                return null;
            }
        }

    }
}
