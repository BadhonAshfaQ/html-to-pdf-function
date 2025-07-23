namespace HtmlToPdfFunctionApp.Utility;


public interface IDapper : IDisposable
{
    DbConnection DbConnection();

    Task<List<T>> GetAll<T>(string sp, DynamicParameters? parameters = null, CommandType commandType = CommandType.StoredProcedure);

    Task<(List<T1>, List<T2>)> GetAll<T1, T2>(string sp, DynamicParameters? parameters = null, CommandType commandType = CommandType.StoredProcedure);
    Task<(List<T1>, List<T2>, List<T3>, List<T4>)> GetAll<T1, T2, T3, T4>(string sp, DynamicParameters? parameters = null, CommandType commandType = CommandType.StoredProcedure);

    Task<T?> Update<T>(string sp, DynamicParameters? parameters = null, CommandType commandType = CommandType.StoredProcedure);
}

public class Dapper : IDapper
{
    public DbConnection DbConnection() => new SqlConnection(ConfigMgr.SqlDbConnection());

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    public Dapper()
    {
        SqlMapper.Settings.CommandTimeout = 240;
    }

    public async Task<List<T>> GetAll<T>(string sp, DynamicParameters? parameters = null, CommandType commandType = CommandType.StoredProcedure)
    {
        using IDbConnection db = DbConnection();
        return (await db.QueryAsync<T>(sp, parameters, commandType: commandType)).ToList();
    }

    public async Task<(List<T1>, List<T2>)> GetAll<T1, T2>(string sp, DynamicParameters? parameters = null, CommandType commandType = CommandType.StoredProcedure)
    {
        using IDbConnection db = DbConnection();
        await using var multi = await db.QueryMultipleAsync(sp, parameters, commandType: commandType);
        return ((await multi.ReadAsync<T1>()).ToList(), (await multi.ReadAsync<T2>()).ToList());
    }

    public async Task<(List<T1>, List<T2>, List<T3>, List<T4>)> GetAll<T1, T2, T3, T4>(string sp, DynamicParameters? parameters = null, CommandType commandType = CommandType.StoredProcedure)
    {
        using IDbConnection db = DbConnection();
        await using var multi = await db.QueryMultipleAsync(sp, parameters, commandType: commandType);
        return ((await multi.ReadAsync<T1>()).ToList(), (await multi.ReadAsync<T2>()).ToList(), (await multi.ReadAsync<T3>()).ToList(), (await multi.ReadAsync<T4>()).ToList());
    }

    public async Task<T?> Update<T>(string sp, DynamicParameters? parameters = null, CommandType commandType = CommandType.StoredProcedure)
    {
        T? result;

        using IDbConnection db = DbConnection();

        try
        {
            if (db.State == ConnectionState.Closed)
                db.Open();

            using var upd = db.BeginTransaction();
            try
            {
                result = await db.QueryFirstOrDefaultAsync<T>(sp, parameters, commandType: commandType, transaction: upd);
                upd.Commit();
            }
            catch (Exception ex)
            {
                upd.Rollback();
                Logger.Error(ex);
                throw;
            }
        }
        finally
        {
            if (db.State == ConnectionState.Open)
                db.Close();
        }

        return result;
    }
}

