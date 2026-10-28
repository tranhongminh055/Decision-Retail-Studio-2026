import os
import sys
import re
import pyodbc

if sys.platform == "win32" and hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

def reencode_sql_files():
    sql_dir = "sql"
    for fname in os.listdir(sql_dir):
        if fname.endswith(".sql"):
            p = os.path.join(sql_dir, fname)
            with open(p, "r", encoding="utf-8") as f:
                content = f.read()
            with open(p, "w", encoding="utf-8-sig") as f:
                f.write(content)
            print(f"-> Saved with UTF-8 BOM: {fname}")

def reseed_database():
    print("\nConnecting to SQL Server (SQLEXPRESS25)...")
    conn = pyodbc.connect("Driver={ODBC Driver 18 for SQL Server};Server=.\\SQLEXPRESS25;Database=DSS_SalesDB;Trusted_Connection=yes;TrustServerCertificate=yes;")
    cursor = conn.cursor()

    tables_to_clear = [
        "DECISION_RESULTS",
        "ALTERNATIVE_SCORES",
        "PAIRWISE_COMPARISONS",
        "ALTERNATIVES",
        "CRITERIA",
        "DECISION_SESSIONS",
        "ORDER_DETAILS",
        "ORDERS",
        "CUSTOMERS",
        "PRODUCTS",
        "CATEGORIES",
        "USERS"
    ]

    print("\nClearing old mojibake data from all tables...")
    for t in tables_to_clear:
        cursor.execute(f"DELETE FROM dbo.{t};")
        try:
            cursor.execute(f"DBCC CHECKIDENT ('dbo.{t}', RESEED, 0);")
        except Exception:
            pass
    conn.commit()
    print("All tables cleared and identity counters reset.")

    # Execute 02_seed_data.sql
    with open("sql/02_seed_data.sql", "r", encoding="utf-8-sig") as f:
        seed_sql = f.read()

    batches = re.split(r"^\s*GO\s*$", seed_sql, flags=re.MULTILINE | re.IGNORECASE)
    print(f"\nExecuting {len(batches)} batches from 02_seed_data.sql...")
    for batch in batches:
        clean_batch = batch.strip()
        if clean_batch and not clean_batch.lower().startswith("use "):
            cursor.execute(clean_batch)
    conn.commit()
    print("-> 02_seed_data.sql executed successfully with pristine Unicode!")

    # Execute 03_dss_reports_and_queries.sql
    with open("sql/03_dss_reports_and_queries.sql", "r", encoding="utf-8-sig") as f:
        views_sql = f.read()

    view_batches = re.split(r"^\s*GO\s*$", views_sql, flags=re.MULTILINE | re.IGNORECASE)
    print(f"\nExecuting {len(view_batches)} batches from 03_dss_reports_and_queries.sql...")
    for batch in view_batches:
        clean_batch = batch.strip()
        if clean_batch and not clean_batch.lower().startswith("use "):
            cursor.execute(clean_batch)
    conn.commit()
    print("-> 03_dss_reports_and_queries.sql views & SP recreated successfully!")

    conn.close()

if __name__ == "__main__":
    reencode_sql_files()
    reseed_database()
