(function () {
    // ── Dữ liệu từ server ──────────────────────────────────
    const dishChart = @Html.Raw(
        System.Text.Json.JsonSerializer.Serialize(
            Model.DishChart ?? new List < ChartCategoryDishResponse > (),
            new System.Text.Json.JsonSerializerOptions {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        }
        )
    );
    // ═══════════════════════════════════════════════
    //  CSV UTILITIES
    // ═══════════════════════════════════════════════
    /**
     * Escape một cell CSV: wrap nếu có dấu phẩy, ngoặc kép, xuống dòng
     */
    function escapeCell(val) {
        if (val === null || val === undefined) return '';
        const str = String(val);
        return /[",\n\r]/.test(str) ? `"${str.replace(/"/g, '""')}"` : str;
    }
    /**
     * Chuyển mảng 2 chiều thành chuỗi CSV
     */
    function toCSV(rows) {
        return rows.map(row => row.map(escapeCell).join(',')).join('\r\n');
    }
    /**
     * Tạo file CSV và trigger download
     * @param {string} filename  - tên file (không cần .csv)
     * @param {string} csvString - nội dung CSV
     */
    function downloadCSV(filename, csvString) {
        // BOM UTF-8 để Excel mở tiếng Việt đúng
        const BOM = '\uFEFF';
        const blob = new Blob([BOM + csvString], { type: 'text/csv;charset=utf-8;' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `${filename}_${formatDateForFile(new Date())}.csv`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    }
    function formatDateForFile(date) {
        return date.toISOString().slice(0, 10).replace(/-/g, '');  // 20250115
    }
    // ═══════════════════════════════════════════════
    //  TOAST NOTIFICATION
    // ═══════════════════════════════════════════════
    let toastTimer = null;
    function showToast(msg, type = 'success') {
        const toast = document.getElementById('exportToast');
        const msgEl = document.getElementById('toastMsg');
        const iconEl = document.getElementById('toastIcon');
        msgEl.textContent = msg;
        toast.className = `export-toast ${type}`;
        iconEl.className = type === 'success'
            ? 'bi bi-check-circle-fill'
            : 'bi bi-x-circle-fill';
        // Force reflow để restart transition
        toast.offsetHeight;
        toast.classList.add('show');
        clearTimeout(toastTimer);
        toastTimer = setTimeout(() => toast.classList.remove('show'), 3000);
    }
    // ═══════════════════════════════════════════════
    //  EXPORT FUNCTIONS
    // ═══════════════════════════════════════════════
    function exportMetrics() {
        if (!metrics) {
            showToast('Không có dữ liệu thông số!', 'error');
            return;
        }
        const rows = [
            ['Thông số', 'Giá trị', 'Thời gian xuất'],
            ['Tổng món ăn', metrics.totalDish, new Date().toLocaleString('vi-VN')],
            ['Tổng người dùng', metrics.totalUser, ''],
            ['Tổng công thức', metrics.totalRecipes, ''],
            ['Tổng đánh giá', metrics.totalReviews, ''],
        ];
        downloadCSV('dashboard_metrics', toCSV(rows));
        showToast('Đã xuất thông số tổng quan ✓');
    }
    function exportChart() {
        if (!dishChart || !dishChart.length) {
            showToast('Không có dữ liệu biểu đồ!', 'error');
            return;
        }
        const total = dishChart.reduce((s, d) => s + d.dishCount, 0);
        const rows = [
            ['STT', 'Danh mục', 'Số món ăn', 'Tỷ lệ (%)', 'Thời gian xuất'],
            ...dishChart
                .slice()
                .sort((a, b) => b.dishCount - a.dishCount)
                .map((d, i) => [
                    i + 1,
                    d.categoryName,
                    d.dishCount,
                    total > 0 ? (d.dishCount / total * 100).toFixed(2) : '0.00',
                    i === 0 ? new Date().toLocaleString('vi-VN') : '',
                ]),
            // Dòng tổng cộng
            ['', 'TỔNG CỘNG', total, '100.00', ''],
        ];
        downloadCSV('dashboard_category_chart', toCSV(rows));
        showToast('Đã xuất biểu đồ danh mục ✓');
    }
    function exportAll() {
        if (!metrics && (!dishChart || !dishChart.length)) {
            showToast('Không có dữ liệu để xuất!', 'error');
            return;
        }
        const now = new Date().toLocaleString('vi-VN');
        const total = dishChart ? dishChart.reduce((s, d) => s + d.dishCount, 0) : 0;
        const rows = [
            // ── Section 1: Metrics ──
            ['=== THÔNG SỐ TỔNG QUAN ===', '', '', ''],
            ['Thông số', 'Giá trị', '', ''],
        ];
        if (metrics) {
            rows.push(
                ['Tổng món ăn', metrics.totalDish, '', ''],
                ['Tổng người dùng', metrics.totalUser, '', ''],
                ['Tổng công thức', metrics.totalRecipes, '', ''],
                ['Tổng đánh giá', metrics.totalReviews, '', ''],
            );
        }
        // ── Section 2: Chart ──
        rows.push(
            ['', '', '', ''],
            ['=== BIỂU ĐỒ DANH MỤC ===', '', '', ''],
            ['STT', 'Danh mục', 'Số món ăn', 'Tỷ lệ (%)'],
        );
        if (dishChart && dishChart.length) {
            dishChart
                .slice()
                .sort((a, b) => b.dishCount - a.dishCount)
                .forEach((d, i) => {
                    rows.push([
                        i + 1,
                        d.categoryName,
                        d.dishCount,
                        total > 0 ? (d.dishCount / total * 100).toFixed(2) : '0.00',
                    ]);
                });
            rows.push(['', 'TỔNG CỘNG', total, '100.00']);
        }
        // ── Footer ──
        rows.push(
            ['', '', '', ''],
            [`Xuất lúc: ${now}`, '', '', ''],
            ['Xuất bởi: FoodAdmin System', '', '', ''],
        );
        downloadCSV('dashboard_full_report', toCSV(rows));
        showToast('Đã xuất toàn bộ báo cáo ✓');
    }
    // ═══════════════════════════════════════════════
    //  EVENT LISTENERS
    // ═══════════════════════════════════════════════
    document.getElementById('exportMetrics')
        ?.addEventListener('click', e => { e.preventDefault(); exportMetrics(); });
    document.getElementById('exportChart')
        ?.addEventListener('click', e => { e.preventDefault(); exportChart(); });
    document.getElementById('exportAll')
        ?.addEventListener('click', e => { e.preventDefault(); exportAll(); });
    if (!dishChart.length) return;

    const labels = dishChart.map(d => d.categoryName);
    const counts = dishChart.map(d => d.dishCount);

    // ── Palette ────────────────────────────────────────────
    const COLORS = [
        '#4f46e5', '#10b981', '#f59e0b', '#ef4444',
        '#06b6d4', '#8b5cf6', '#ec4899', '#14b8a6',
        '#f97316', '#84cc16'
    ];
    const palette = labels.map((_, i) => COLORS[i % COLORS.length]);

    // ── Shared defaults ────────────────────────────────────
    Chart.defaults.font.family = "'Inter','Segoe UI',sans-serif";
    Chart.defaults.plugins.legend.labels.boxWidth = 12;
    Chart.defaults.plugins.legend.labels.padding = 16;

    // ── Bar Chart ──────────────────────────────────────────
    new Chart(document.getElementById('barChart'), {
        type: 'bar',
        data: {
            labels,
            datasets: [{
                label: 'Số món ăn',
                data: counts,
                backgroundColor: palette,
                borderRadius: 8,
                borderSkipped: false,
                maxBarThickness: 52
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: { display: false },
                tooltip: {
                    callbacks: {
                        label: ctx => ` ${ctx.parsed.y.toLocaleString()} món`
                    }
                }
            },
            scales: {
                x: { grid: { display: false } },
                y: {
                    beginAtZero: true,
                    grid: { color: '#f1f5f9' },
                    ticks: {
                        precision: 0,
                        callback: v => v.toLocaleString()
                    }
                }
            }
        }
    });

    // ── Doughnut Chart ─────────────────────────────────────
    new Chart(document.getElementById('doughnutChart'), {
        type: 'doughnut',
        data: {
            labels,
            datasets: [{
                data: counts,
                backgroundColor: palette,
                borderWidth: 2,
                borderColor: '#ffffff',
                hoverOffset: 8
            }]
        },
        options: {
            responsive: true,
            cutout: '65%',
            plugins: {
                legend: { position: 'bottom' },
                tooltip: {
                    callbacks: {
                        label: ctx => {
                            const total = ctx.dataset.data.reduce((a, b) => a + b, 0);
                            const pct = (ctx.parsed / total * 100).toFixed(1);
                            return ` ${ctx.label}: ${ctx.parsed.toLocaleString()} (${pct}%)`;
                        }
                    }
                }
            }
        }
    });
})();