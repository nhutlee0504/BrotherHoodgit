window.onload = () => {
    var ctx = document.getElementById('orderPieChart').getContext('2d');
    var pieChart = new Chart(ctx, {
        type: 'pie', // Đổi loại biểu đồ thành tròn
        data: {
            labels: ['Tổng số đơn hàng', 'Đã hủy', 'Đã duyệt'], // Các nhãn cho biểu đồ
            datasets: [{
                label: 'Đơn hàng',
                data: [
                    @statistics.TotalOrders,  // Dữ liệu cho Tổng số đơn hàng
                    @statistics.CanceledOrders, // Dữ liệu cho Đã hủy
                    @statistics.CompletedOrders  // Dữ liệu cho Đã duyệt
                ],
                backgroundColor: [
                    'rgba(0, 123, 255, 0.7)', // Màu cho Tổng số đơn hàng
                    'rgba(220, 53, 69, 0.7)',  // Màu cho Đã hủy
                    'rgba(40, 167, 69, 0.7)'   // Màu cho Đã duyệt
                ],
                borderColor: [
                    'rgba(0, 123, 255, 1)',  // Màu viền cho Tổng số đơn hàng
                    'rgba(220, 53, 69, 1)',   // Màu viền cho Đã hủy
                    'rgba(40, 167, 69, 1)'    // Màu viền cho Đã duyệt
                ],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'top', // Hiển thị chú thích ở trên
                    labels: {
                        font: {
                            size: 14 // Kích thước phông chữ của chú thích
                        }
                    }
                },
                tooltip: {
                    enabled: true // Bật tooltip khi hover vào phần biểu đồ
                }
            }
        }
    });
};