# -*- coding: utf-8 -*-
"""
Script ve So do Thuc the Moi quan he (ERD - Entity Relationship Diagram)
cho Co so du lieu DSS_SalesDB (12 bang chuan 3NF)
IS 402 A - Nhom 05 - Duy Tan University
"""

import matplotlib.pyplot as plt
import matplotlib.patches as patches
import os

def generate_erd_diagram(output_path="docs/images/sodo_thuc_the_erd.png"):
    plt.rcParams['font.family'] = 'sans-serif'
    plt.rcParams['font.sans-serif'] = ['Segoe UI', 'Arial', 'Tahoma', 'DejaVu Sans']
    
    fig, ax = plt.subplots(figsize=(18, 12.5), dpi=300)
    ax.set_xlim(0, 100)
    ax.set_ylim(0, 100)
    ax.axis('off')
    
    fig.patch.set_facecolor('#FFFFFF')
    ax.set_facecolor('#FFFFFF')
    
    # Title Banner
    banner = patches.FancyBboxPatch((2, 92.5), 96, 6.8, boxstyle="round,pad=0.2,rounding_size=0.5",
                                    facecolor="#0F172A", edgecolor="#1E293B", linewidth=1.5)
    ax.add_patch(banner)
    ax.text(50, 96.5, "SƠ ĐỒ THỰC THỂ QUAN HỆ CƠ SỞ DỮ LIỆU CHUẨN 3NF (ERD — DSS_SALESDB)", 
            color="#FFFFFF", fontsize=14, weight='bold', ha='center', va='center')
    ax.text(50, 94.0, "HỆ THỐNG HỖ TRỢ RA QUYẾT ĐỊNH BÁN LẺ 2026 • 12 THỰC THỂ LIÊN KẾT 2 PHÂN HỆ OLTP & DSS", 
            color="#94A3B8", fontsize=9.2, weight='normal', ha='center', va='center')

    # Subgraph Boundaries
    # Left Subgraph: OLTP Module (Sales & Warehouse)
    bg_oltp = patches.FancyBboxPatch((2, 3), 46, 88, boxstyle="round,pad=0.4,rounding_size=0.6",
                                     facecolor="#F8FAFC", edgecolor="#CBD5E1", linewidth=1.5, linestyle="--")
    ax.add_patch(bg_oltp)
    ax.text(25, 89.2, "PHÂN HỆ VẬN HÀNH BÁN LẺ & KHO (OLTP MODULE)", color="#0369A1", fontsize=10.5, weight='bold', ha='center')

    # Right Subgraph: DSS Module (AHP Decision Engine)
    bg_dss = patches.FancyBboxPatch((52, 3), 46, 88, boxstyle="round,pad=0.4,rounding_size=0.6",
                                    facecolor="#FDF4FF", edgecolor="#E9D5FF", linewidth=1.5, linestyle="--")
    ax.add_patch(bg_dss)
    ax.text(75, 89.2, "PHÂN HỆ HỖ TRỢ RA QUYẾT ĐỊNH (DSS AHP MODULE)", color="#7E22CE", fontsize=10.5, weight='bold', ha='center')

    # Helper function to draw an entity table card
    def draw_entity(x, y, w, h, title, fields, header_color="#1E293B", border_color="#334155", title_color="#FFFFFF"):
        # Header Box
        header_h = 2.8
        card = patches.FancyBboxPatch((x, y), w, h, boxstyle="round,pad=0.15,rounding_size=0.3",
                                      facecolor="#FFFFFF", edgecolor=border_color, linewidth=1.4, zorder=2)
        ax.add_patch(card)
        
        # Header rect
        hdr = patches.FancyBboxPatch((x, y + h - header_h), w, header_h, boxstyle="round,pad=0.15,rounding_size=0.3",
                                     facecolor=header_color, edgecolor=border_color, linewidth=1.4, zorder=3)
        ax.add_patch(hdr)
        ax.text(x + w/2, y + h - header_h/2, title, color=title_color, fontsize=8.2, weight='bold', ha='center', va='center', zorder=4)
        
        # Fields
        cur_y = y + h - header_h - 1.5
        for f in fields:
            is_pk = "[PK]" in f
            is_fk = "[FK]" in f
            f_color = "#B91C1C" if is_pk else ("#1D4ED8" if is_fk else "#334155")
            f_weight = 'bold' if (is_pk or is_fk) else 'normal'
            ax.text(x + 0.8, cur_y, f, color=f_color, fontsize=6.8, weight=f_weight, va='center', zorder=4)
            cur_y -= 1.65

    # -------------------------------------------------------------------------
    # ENTITIES: OLTP MODULE (Left)
    # -------------------------------------------------------------------------
    # 1. USERS (Top Left)
    users_fields = [
        "[PK] user_id : INT IDENTITY",
        "[UQ] username : VARCHAR(50)",
        "password_hash : VARCHAR(255)",
        "full_name : NVARCHAR(100)",
        "[UQ] email : VARCHAR(100)",
        "role : VARCHAR(20) [CHECK]",
        "status : BIT (DEFAULT 1)"
    ]
    draw_entity(4, 72, 20, 15, "dbo.USERS", users_fields, header_color="#0F172A", border_color="#334155")

    # 2. CATEGORIES (Middle Left)
    cat_fields = [
        "[PK] category_id : INT IDENTITY",
        "[UQ] category_code : VARCHAR(30)",
        "category_name : NVARCHAR(100)",
        "description : NVARCHAR(500)",
        "status : BIT"
    ]
    draw_entity(4, 52, 20, 12.5, "dbo.CATEGORIES", cat_fields, header_color="#0369A1", border_color="#0284C7")

    # 3. PRODUCTS (Bottom Left)
    prod_fields = [
        "[PK] product_id : INT IDENTITY",
        "[FK] category_id : INT",
        "[UQ] product_code : VARCHAR(50)",
        "product_name : NVARCHAR(200)",
        "cost_price : DECIMAL(18,2)",
        "selling_price : DECIMAL(18,2)",
        "stock_quantity : INT",
        "lead_time_days : INT"
    ]
    draw_entity(4, 25, 20, 19, "dbo.PRODUCTS", prod_fields, header_color="#0369A1", border_color="#0284C7")

    # 4. CUSTOMERS (Top Middle-Left)
    cust_fields = [
        "[PK] customer_id : INT IDENTITY",
        "[UQ] customer_code : VARCHAR(30)",
        "customer_name : NVARCHAR(150)",
        "[UQ] phone : VARCHAR(20)",
        "customer_segment : VARCHAR(20)",
        "total_spent : DECIMAL(18,2)"
    ]
    draw_entity(26, 72, 20, 14, "dbo.CUSTOMERS", cust_fields, header_color="#15803D", border_color="#16A34A")

    # 5. ORDERS (Middle Center-Left)
    orders_fields = [
        "[PK] order_id : INT IDENTITY",
        "[UQ] order_code : VARCHAR(50)",
        "[FK] customer_id : INT",
        "[FK] user_id : INT",
        "order_date : DATETIME2",
        "order_status : VARCHAR(20)",
        "total_amount : DECIMAL(18,2)"
    ]
    draw_entity(26, 49, 20, 16.5, "dbo.ORDERS", orders_fields, header_color="#15803D", border_color="#16A34A")

    # 6. ORDER_DETAILS (Bottom Center-Left)
    od_fields = [
        "[PK] order_detail_id : INT",
        "[FK] order_id : INT",
        "[FK] product_id : INT",
        "quantity : INT [CHECK > 0]",
        "unit_price : DECIMAL(18,2)",
        "discount_rate : DECIMAL(5,2)",
        "line_total : DECIMAL(18,2)"
    ]
    draw_entity(26, 25, 20, 16.5, "dbo.ORDER_DETAILS", od_fields, header_color="#15803D", border_color="#16A34A")

    # -------------------------------------------------------------------------
    # ENTITIES: DSS AHP MODULE (Right)
    # -------------------------------------------------------------------------
    # 7. DECISION_SESSIONS (Top Center-Right)
    session_fields = [
        "[PK] session_id : INT IDENTITY",
        "[UQ] session_code : VARCHAR(50)",
        "session_title : NVARCHAR(200)",
        "[FK] created_by : INT (USERS)",
        "consistency_ratio : DECIMAL(8,4)",
        "is_consistent : BIT PERSISTED",
        "status : VARCHAR(20)"
    ]
    draw_entity(54, 72, 20, 15, "dbo.DECISION_SESSIONS", session_fields, header_color="#4F46E5", border_color="#4338CA")

    # 8. CRITERIA (Top Far-Right)
    crit_fields = [
        "[PK] criterion_id : INT IDENTITY",
        "[FK] parent_id : INT NULL",
        "[UQ] criterion_code : VARCHAR(30)",
        "criterion_name : NVARCHAR(150)",
        "criterion_type : VARCHAR(10)",
        "data_source_type : VARCHAR(30)",
        "is_active : BIT"
    ]
    draw_entity(76, 72, 20, 15, "dbo.CRITERIA", crit_fields, header_color="#7E22CE", border_color="#6B21A8")

    # 9. ALTERNATIVES (Middle Center-Right)
    alt_fields = [
        "[PK] alternative_id : INT IDENTITY",
        "[FK] session_id : INT",
        "[FK] product_id : INT",
        "[UQ] alternative_code : VARCHAR(50)",
        "alternative_name : NVARCHAR(200)",
        "description : NVARCHAR(1000)"
    ]
    draw_entity(54, 49, 20, 15.5, "dbo.ALTERNATIVES", alt_fields, header_color="#4F46E5", border_color="#4338CA")

    # 10. PAIRWISE_COMPARISONS (Middle Far-Right)
    pair_fields = [
        "[PK] comparison_id : INT",
        "[FK] session_id : INT",
        "[FK] criterion_id_1 : INT",
        "[FK] criterion_id_2 : INT",
        "saaty_value : DECIMAL(10,5)",
        "[FK] evaluated_by : INT (USERS)"
    ]
    draw_entity(76, 49, 20, 15.5, "dbo.PAIRWISE_COMPARISONS", pair_fields, header_color="#7E22CE", border_color="#6B21A8")

    # 11. ALTERNATIVE_SCORES (Bottom Center-Right)
    score_fields = [
        "[PK] score_id : INT IDENTITY",
        "[FK] session_id : INT",
        "[FK] alternative_id : INT",
        "[FK] criterion_id : INT",
        "raw_value : DECIMAL(18,4)",
        "normalized_score : DECIMAL(10,6)"
    ]
    draw_entity(54, 25, 20, 15.5, "dbo.ALTERNATIVE_SCORES", score_fields, header_color="#4F46E5", border_color="#4338CA")

    # 12. DECISION_RESULTS (Bottom Far-Right)
    res_fields = [
        "[PK] result_id : INT IDENTITY",
        "[FK] session_id : INT",
        "[FK] alternative_id : INT",
        "final_score : DECIMAL(10,6)",
        "ranking : INT [DENSE_RANK]",
        "decision_outcome : VARCHAR(30)",
        "decision_date : DATETIME2"
    ]
    draw_entity(76, 25, 20, 16.5, "dbo.DECISION_RESULTS", res_fields, header_color="#B91C1C", border_color="#991B1B")

    # -------------------------------------------------------------------------
    # RELATIONSHIP CONNECTORS & CARDINALITIES
    # -------------------------------------------------------------------------
    # CATEGORIES -> PRODUCTS (1:N)
    ax.annotate("", xy=(14, 44), xytext=(14, 52), arrowprops=dict(arrowstyle="-|>", color="#0284C7", lw=1.6))
    ax.text(14.5, 48, "1 : N", color="#0284C7", fontsize=7.2, weight='bold')

    # PRODUCTS -> ORDER_DETAILS (1:N)
    ax.annotate("", xy=(26, 33), xytext=(24, 33), arrowprops=dict(arrowstyle="-|>", color="#0284C7", lw=1.6))
    ax.text(24.5, 34, "1 : N", color="#0284C7", fontsize=7.2, weight='bold')

    # CUSTOMERS -> ORDERS (1:N)
    ax.annotate("", xy=(36, 65.5), xytext=(36, 72), arrowprops=dict(arrowstyle="-|>", color="#16A34A", lw=1.6))
    ax.text(36.5, 68, "1 : N", color="#16A34A", fontsize=7.2, weight='bold')

    # ORDERS -> ORDER_DETAILS (1:N)
    ax.annotate("", xy=(36, 41.5), xytext=(36, 49), arrowprops=dict(arrowstyle="-|>", color="#16A34A", lw=1.6))
    ax.text(36.5, 45, "1 : N", color="#16A34A", fontsize=7.2, weight='bold')

    # USERS -> ORDERS (1:N)
    ax.annotate("", xy=(26, 60), xytext=(20, 72), arrowprops=dict(arrowstyle="-|>", color="#64748B", lw=1.4, linestyle=':'))
    ax.text(23, 66, "1 : N", color="#64748B", fontsize=7.2, weight='bold')

    # USERS -> DECISION_SESSIONS (1:N)
    ax.annotate("", xy=(54, 78), xytext=(24, 78), arrowprops=dict(arrowstyle="-|>", color="#4F46E5", lw=1.8, linestyle='--'))
    ax.text(39, 79, "1 : N (created_by)", color="#4F46E5", fontsize=7.2, weight='bold', ha='center')

    # PRODUCTS -> ALTERNATIVES (Bridge OLTP -> DSS)
    ax.annotate("", xy=(54, 55), xytext=(24, 40), arrowprops=dict(arrowstyle="-|>", color="#D97706", lw=2.0))
    ax.text(39, 48, "1 : N (Ánh xạ SKU ứng viên)", color="#D97706", fontsize=7.5, weight='bold', ha='center')

    # DECISION_SESSIONS -> ALTERNATIVES (1:N)
    ax.annotate("", xy=(64, 64.5), xytext=(64, 72), arrowprops=dict(arrowstyle="-|>", color="#4F46E5", lw=1.6))
    ax.text(64.5, 68, "1 : N", color="#4F46E5", fontsize=7.2, weight='bold')

    # DECISION_SESSIONS -> PAIRWISE_COMPARISONS (1:N)
    ax.annotate("", xy=(76, 60), xytext=(72, 72), arrowprops=dict(arrowstyle="-|>", color="#4F46E5", lw=1.6))
    ax.text(74.5, 66, "1 : N", color="#4F46E5", fontsize=7.2, weight='bold')

    # CRITERIA -> PAIRWISE_COMPARISONS (1:N x2)
    ax.annotate("", xy=(86, 64.5), xytext=(86, 72), arrowprops=dict(arrowstyle="-|>", color="#7E22CE", lw=1.6))
    ax.text(86.5, 68, "1 : N (Crit 1, 2)", color="#7E22CE", fontsize=7.2, weight='bold')

    # ALTERNATIVES -> ALTERNATIVE_SCORES (1:N)
    ax.annotate("", xy=(64, 40.5), xytext=(64, 49), arrowprops=dict(arrowstyle="-|>", color="#4F46E5", lw=1.6))
    ax.text(64.5, 45, "1 : N", color="#4F46E5", fontsize=7.2, weight='bold')

    # ALTERNATIVES -> DECISION_RESULTS (1:1)
    ax.annotate("", xy=(76, 33), xytext=(74, 52), arrowprops=dict(arrowstyle="-|>", color="#B91C1C", lw=1.8))
    ax.text(75.5, 43, "1 : 1", color="#B91C1C", fontsize=7.5, weight='bold')

    # Footnote / Legend
    ax.text(50, 1.2, "[*] QUY ƯỚC ERD: [PK] = Khóa chính • [FK] = Khóa ngoại tham chiếu • [UQ] = Ràng buộc duy nhất • [CHECK] = Ràng buộc miền giá trị",
            color="#475569", fontsize=8.2, style='italic', ha='center')

    plt.tight_layout()
    os.makedirs(os.path.dirname(output_path), exist_ok=True)
    plt.savefig(output_path, facecolor=fig.get_facecolor(), edgecolor='none')
    plt.close()
    print(f"ERD diagram successfully created at: {output_path}")

if __name__ == "__main__":
    generate_erd_diagram()
