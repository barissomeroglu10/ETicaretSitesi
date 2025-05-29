// Site JavaScript

// Toastr Configuration
toastr.options = {
    closeButton: true,
    progressBar: true,
    positionClass: "toast-top-right",
    timeOut: 3000
};

// Form Validation
document.addEventListener('DOMContentLoaded', function() {
    const forms = document.querySelectorAll('.needs-validation');
    Array.from(forms).forEach(form => {
        form.addEventListener('submit', event => {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            form.classList.add('was-validated');
        }, false);
    });
});

// Image Preview
function previewImage(input, previewId) {
    const preview = document.getElementById(previewId);
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = function(e) {
            preview.src = e.target.result;
            preview.style.display = 'block';
        }
        reader.readAsDataURL(input.files[0]);
    }
}

// Quantity Input
function updateQuantity(input, action) {
    const currentValue = parseInt(input.value);
    if (action === 'increase') {
        input.value = currentValue + 1;
    } else if (action === 'decrease' && currentValue > 1) {
        input.value = currentValue - 1;
    }
    input.dispatchEvent(new Event('change'));
}

// Price Format
function formatPrice(price) {
    return new Intl.NumberFormat('tr-TR', {
        style: 'currency',
        currency: 'TRY'
    }).format(price);
}

// Cart Functions
function addToCart(productId, quantity = 1) {
    fetch('/Sepet/Ekle', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            urunId: productId,
            miktar: quantity
        })
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            toastr.success('Ürün sepete eklendi');
            updateCartCount(data.cartCount);
        } else {
            toastr.error(data.message || 'Bir hata oluştu');
        }
    })
    .catch(error => {
        console.error('Error:', error);
        toastr.error('Bir hata oluştu');
    });
}

function updateCartCount(count) {
    const cartCountElement = document.getElementById('cartCount');
    if (cartCountElement) {
        cartCountElement.textContent = count;
    }
}

function removeFromCart(productId) {
    if (confirm('Bu ürünü sepetten çıkarmak istediğinizden emin misiniz?')) {
        fetch('/Sepet/Cikar', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                urunId: productId
            })
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                toastr.success('Ürün sepetten çıkarıldı');
                updateCartCount(data.cartCount);
                location.reload();
            } else {
                toastr.error(data.message || 'Bir hata oluştu');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            toastr.error('Bir hata oluştu');
        });
    }
}

// Search Function
function searchProducts(query) {
    if (query.length >= 2) {
        fetch(`/Urun/Ara?q=${encodeURIComponent(query)}`)
            .then(response => response.json())
            .then(data => {
                const resultsContainer = document.getElementById('searchResults');
                if (resultsContainer) {
                    resultsContainer.innerHTML = '';
                    if (data.length > 0) {
                        data.forEach(product => {
                            const productElement = document.createElement('div');
                            productElement.className = 'search-result-item';
                            productElement.innerHTML = `
                                <a href="/Urun/Detay/${product.id}">
                                    <img src="${product.gorselUrl}" alt="${product.ad}">
                                    <div class="search-result-info">
                                        <h6>${product.ad}</h6>
                                        <p>${formatPrice(product.fiyat)}</p>
                                    </div>
                                </a>
                            `;
                            resultsContainer.appendChild(productElement);
                        });
                        resultsContainer.style.display = 'block';
                    } else {
                        resultsContainer.innerHTML = '<div class="no-results">Sonuç bulunamadı</div>';
                        resultsContainer.style.display = 'block';
                    }
                }
            })
            .catch(error => {
                console.error('Error:', error);
            });
    } else {
        const resultsContainer = document.getElementById('searchResults');
        if (resultsContainer) {
            resultsContainer.style.display = 'none';
        }
    }
}

// Close search results when clicking outside
document.addEventListener('click', function(event) {
    const searchContainer = document.querySelector('.search-container');
    const searchResults = document.getElementById('searchResults');
    if (searchContainer && !searchContainer.contains(event.target) && searchResults) {
        searchResults.style.display = 'none';
    }
});

// Admin Panel JavaScript
$(document).ready(function() {
    // Sidebar Toggle
    $("#menu-toggle").click(function(e) {
        e.preventDefault();
        $("#wrapper").toggleClass("toggled");
    });

    // DataTables Initialization
    if ($.fn.DataTable) {
        $('.datatable').DataTable({
            language: {
                url: '//cdn.datatables.net/plug-ins/1.10.24/i18n/Turkish.json'
            },
            responsive: true
        });
    }

    // Tooltips Initialization
    if ($.fn.tooltip) {
        $('[data-toggle="tooltip"]').tooltip();
    }

    // Confirm Delete
    $('.btn-delete').click(function(e) {
        if (!confirm('Bu öğeyi silmek istediğinizden emin misiniz?')) {
            e.preventDefault();
        }
    });

    // Image Preview
    $('.custom-file-input').on('change', function() {
        let fileName = $(this).val().split('\\').pop();
        $(this).next('.custom-file-label').addClass("selected").html(fileName);
        
        if (this.files && this.files[0]) {
            let reader = new FileReader();
            reader.onload = function(e) {
                $('#imagePreview').attr('src', e.target.result);
            }
            reader.readAsDataURL(this.files[0]);
        }
    });

    // Form Validation
    if ($.fn.validate) {
        $("form").validate({
            errorElement: 'span',
            errorClass: 'invalid-feedback',
            validClass: 'valid-feedback',
            highlight: function(element, errorClass, validClass) {
                $(element).addClass('is-invalid').removeClass('is-valid');
            },
            unhighlight: function(element, errorClass, validClass) {
                $(element).addClass('is-valid').removeClass('is-invalid');
            }
        });
    }

    // Select2 Initialization
    if ($.fn.select2) {
        $('.select2').select2({
            theme: 'bootstrap4',
            language: 'tr'
        });
    }

    // Datepicker Initialization
    if ($.fn.datepicker) {
        $('.datepicker').datepicker({
            format: 'dd/mm/yyyy',
            language: 'tr',
            autoclose: true,
            todayHighlight: true
        });
    }

    // Auto-hide alerts after 5 seconds
    setTimeout(function() {
        $('.alert').fadeOut('slow');
    }, 5000);

    // AJAX form submission
    $('.ajax-form').on('submit', function(e) {
        e.preventDefault();
        let form = $(this);
        let url = form.attr('action');
        let method = form.attr('method');
        let data = form.serialize();

        $.ajax({
            url: url,
            type: method,
            data: data,
            success: function(response) {
                if (response.success) {
                    showAlert('success', response.message);
                    if (response.redirect) {
                        setTimeout(function() {
                            window.location.href = response.redirect;
                        }, 1000);
                    }
                } else {
                    showAlert('danger', response.message);
                }
            },
            error: function(xhr) {
                showAlert('danger', 'Bir hata oluştu. Lütfen tekrar deneyin.');
            }
        });
    });

    // Show alert function
    function showAlert(type, message) {
        let alert = $('<div class="alert alert-' + type + ' alert-dismissible fade show" role="alert">' +
            message +
            '<button type="button" class="close" data-dismiss="alert" aria-label="Close">' +
            '<span aria-hidden="true">&times;</span>' +
            '</button>' +
            '</div>');

        $('.alert-container').append(alert);
        setTimeout(function() {
            alert.alert('close');
        }, 5000);
    }

    // Initialize all tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    });

    // Initialize all popovers
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'))
    var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl)
    });
}); 