// Product page logic with dynamic variant selection
let currentVariant = null;
let currentPrice = window.productData.basePrice;

// Quantity controls
function increaseQuantity() {
    const input = document.getElementById("quantity");
    input.value = parseInt(input.value) + 1;
    updateTotal();
    hideReceipt();
}

function decreaseQuantity() {
    const input = document.getElementById("quantity");
    if (parseInt(input.value) > 1) {
        input.value = parseInt(input.value) - 1;
        updateTotal();
        hideReceipt();
    }
}

function updateTotal() {
    const quantity = parseInt(document.getElementById("quantity").value);
    const subtotal = currentPrice * quantity;
    const deliveryCharge = subtotal < 1999 ? 100 : 0;
    const total = subtotal + deliveryCharge;

    document.getElementById("subtotal").textContent = `৳ ${subtotal}`;
    document.getElementById("total").textContent = `৳ ${total}`;

    const receipt = document.getElementById("receipt");
    if (receipt && receipt.style.display !== "none") {
        document.getElementById("receipt-subtotal").textContent = `৳ ${subtotal}`;
        document.getElementById("delivery-charge").textContent = `৳ ${deliveryCharge}`;
        document.getElementById("receipt-total").textContent = `৳ ${total}`;
        document.getElementById("receipt-quantity").textContent = quantity;
    }
}

function scrollToOrder() {
    document.getElementById("orderSection").scrollIntoView({ behavior: "smooth" });
}

function scrollToTop() {
    window.scrollTo({ top: 0, behavior: "smooth" });
}

function hideReceipt() {
    const receipt = document.getElementById("receipt");
    receipt.style.display = "none";
}

// Build variant name dynamically from selected attributes
function buildVariantName() {
    const selectors = document.querySelectorAll('.variant-selector');
    const parts = [];

    selectors.forEach(selector => {
        const selectedOption = selector.options[selector.selectedIndex];
        if (selectedOption && selectedOption.value) {
            parts.push(selectedOption.textContent);
        }
    });

    if (parts.length > 0) {
        // Get product name from the page or window data
        const productName = window.productData.productName || document.querySelector('.price-card h3')?.textContent || '';
        return `${productName} - ${parts.join(' - ')}`;
    }
    return window.productData.productName;
}

// Variant selection logic
function findMatchingVariant() {
    const selectors = document.querySelectorAll('.variant-selector');
    const selectedAttributes = {};

    // Collect all selected attribute values
    let allSelected = true;
    selectors.forEach(selector => {
        const attrId = parseInt(selector.dataset.attributeId);
        const valueId = parseInt(selector.value);

        if (!valueId) {
            allSelected = false;
            return;
        }

        selectedAttributes[attrId] = valueId;
    });

    if (!allSelected) {
        return null;
    }

    // Find variant that matches all selected attributes
    const variants = window.productData.variants;
    const variantAttributeValues = window.productData.variantAttributeValues;

    for (let variant of variants) {
        // Get all attribute values for this variant
        const variantAttrs = variantAttributeValues.filter(vav => vav.VariantId === variant.Id);

        // Check if all selected attributes match
        let matches = true;
        for (let attrId in selectedAttributes) {
            const expectedValueId = selectedAttributes[attrId];
            const hasMatch = variantAttrs.some(vav =>
                vav.AttributeId === parseInt(attrId) && vav.AttributeValueId === expectedValueId
            );

            if (!hasMatch) {
                matches = false;
                break;
            }
        }

        if (matches && variantAttrs.length === Object.keys(selectedAttributes).length) {
            return variant;
        }
    }

    return null;
}

function updateVariantSelection() {
    const variant = findMatchingVariant();

    if (variant) {
        currentVariant = variant;

        // Get price from VariantPriceStocks
        const priceStock = window.productData.variantPriceStocks.find(vps => vps.VariantId === variant.Id);

        if (priceStock) {
            currentPrice = priceStock.Price || window.productData.basePrice;
        } else {
            currentPrice = variant.VariantPrice || window.productData.basePrice;
        }

        // Update UI - Use the variant name from the database
        document.getElementById("selectedVariantId").value = variant.Id;
        document.getElementById("selectedVariantPrice").value = currentPrice;
        document.getElementById("displayPrice").textContent = `${currentPrice}`;
        document.getElementById("selectedVariantName").textContent = variant.VariantName;

        // Update variant images if available
        const variantImages = window.productData.variantImages[variant.Id];
        if (variantImages && variantImages.length > 0) {
            console.log("Variant images:", variantImages);
        }

        updateTotal();

        // Show availability message
        showVariantAvailability(priceStock);
    } else {
        // Reset to base price if no variant matches
        currentPrice = window.productData.basePrice;
        document.getElementById("selectedVariantId").value = "";
        document.getElementById("displayPrice").textContent = `${currentPrice}`;
        document.getElementById("selectedVariantName").textContent = window.productData.productName;
        updateTotal();

        // Check if all options are selected but variant doesn't exist
        const selectors = document.querySelectorAll('.variant-selector');
        let allSelected = true;
        selectors.forEach(selector => {
            if (!selector.value) {
                allSelected = false;
            }
        });

        if (allSelected) {
            showVariantUnavailable();
        }
    }
}

function showVariantAvailability(priceStock) {
    // Remove any existing availability messages
    const existingMsg = document.querySelector('.availability-message');
    if (existingMsg) existingMsg.remove();

    if (!priceStock) return;

    const priceCard = document.querySelector('.price-card');
    const msgDiv = document.createElement('div');
    msgDiv.className = 'availability-message';

    if (priceStock.StockQty > 0) {
        msgDiv.innerHTML = `<span style="color: #28a745;">✓ স্টকে আছে (${priceStock.StockQty} টি)</span>`;
    } else if (priceStock.AllowBackorder) {
        msgDiv.innerHTML = `<span style="color: #ffc107;">⚠ প্রি-অর্ডার করুন (স্টক শেষ)</span>`;
    } else {
        msgDiv.innerHTML = `<span style="color: #dc3545;">✗ স্টক শেষ</span>`;
    }

    msgDiv.style.marginTop = '10px';
    msgDiv.style.fontSize = '14px';
    msgDiv.style.fontWeight = 'bold';
    priceCard.appendChild(msgDiv);
}

function showVariantUnavailable() {
    // Remove any existing availability messages
    const existingMsg = document.querySelector('.availability-message');
    if (existingMsg) existingMsg.remove();

    const priceCard = document.querySelector('.price-card');
    const msgDiv = document.createElement('div');
    msgDiv.className = 'availability-message';
    msgDiv.innerHTML = `<span style="color: #dc3545;">✗ এই কম্বিনেশন উপলব্ধ নেই</span>`;
    msgDiv.style.marginTop = '10px';
    msgDiv.style.fontSize = '14px';
    msgDiv.style.fontWeight = 'bold';
    priceCard.appendChild(msgDiv);
}

function submitOrder() {
    const name = document.getElementById("name").value.trim();
    const phone = document.getElementById("phone").value.trim();
    const address = document.getElementById("address").value.trim();
    const quantity = parseInt(document.getElementById("quantity").value);
    const variantId = document.getElementById("selectedVariantId").value;

    if (!name || !phone || !address) {
        alert("❌ অনুগ্রহ করে আপনার নাম, মোবাইল নাম্বার এবং ঠিকানা পূরণ করুন।");
        return;
    }

    // Check if variant is selected
    const selectors = document.querySelectorAll('.variant-selector');
    let allSelected = true;
    selectors.forEach(selector => {
        if (!selector.value) {
            allSelected = false;
        }
    });

    if (!allSelected) {
        alert("❌ অনুগ্রহ করে সকল অপশন নির্বাচন করুন।");
        return;
    }

    if (!variantId) {
        alert("❌ দুঃখিত, এই কম্বিনেশন উপলব্ধ নেই। অনুগ্রহ করে অন্য অপশন নির্বাচন করুন।");
        return;
    }

    // Check stock availability
    const priceStock = window.productData.variantPriceStocks.find(vps => vps.VariantId === parseInt(variantId));

    if (priceStock && priceStock.TrackInventory) {
        if (priceStock.StockQty < quantity) {
            if (!priceStock.AllowBackorder) {
                alert(`❌ দুঃখিত, শুধুমাত্র ${priceStock.StockQty} টি স্টকে আছে।`);
                return;
            } else {
                if (!confirm(`⚠ স্টকে ${priceStock.StockQty} টি আছে। আপনি ${quantity} টি প্রি-অর্ডার করতে চান?`)) {
                    return;
                }
            }
        }
    }

    if (!/^\d{11}$/.test(phone)) {
        alert("❌ অনুগ্রহ করে একটি সঠিক ১১ সংখ্যার মোবাইল নাম্বার দিন।");
        return;
    }

    const subtotal = currentPrice * quantity;
    const deliveryCharge = subtotal < 1999 ? 100 : 0;
    const total = subtotal + deliveryCharge;

    // Use the variant name from the UI or build it
    const variantNameElement = document.getElementById("selectedVariantName");
    const variantName = currentVariant ? currentVariant.VariantName : buildVariantName();

    document.getElementById("receipt-subtotal").textContent = `৳ ${subtotal}`;
    document.getElementById("delivery-charge").textContent = `৳ ${deliveryCharge}`;
    document.getElementById("receipt-total").textContent = `৳ ${total}`;
    document.getElementById("receipt-product").textContent = variantName;
    document.getElementById("receipt-name").textContent = name;
    document.getElementById("receipt-phone").textContent = phone;
    document.getElementById("receipt-address").textContent = address;
    document.getElementById("receipt-quantity").textContent = quantity;

    const receipt = document.getElementById("receipt");
    receipt.style.display = "block";
    receipt.scrollIntoView({ behavior: "smooth" });

    alert("✓ অর্ডার সম্পন্ন হয়েছে! আমাদের প্রতিনিধি শীঘ্রই যোগাযোগ করবে।");

    // Optional: Send order to backend
    sendOrderToBackend({
        variantId,
        name,
        phone,
        address,
        quantity,
        total,
        variantName,
        price: currentPrice
    });
}

// Scroll top button visibility
window.addEventListener("scroll", () => {
    const scrollTop = document.getElementById("scrollTop");
    scrollTop.classList.toggle("visible", window.scrollY > 300);
});

// Initialize
window.addEventListener('DOMContentLoaded', () => {
    updateTotal();

    // Add event listeners to variant selectors
    const selectors = document.querySelectorAll('.variant-selector');
    selectors.forEach(selector => {
        selector.addEventListener('change', () => {
            updateVariantSelection();
            hideReceipt();
        });
    });

    // Add event listeners to form elements
    const orderFormElements = document.querySelectorAll('.order-form input, .order-form textarea, .order-form select');
    orderFormElements.forEach(el => {
        el.addEventListener('input', hideReceipt);
        el.addEventListener('change', hideReceipt);
    });
});

// Optional: Function to send order to backend
function sendOrderToBackend(orderData) {
    fetch('/api/orders', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(orderData)
    })
        .then(response => response.json())
        .then(data => {
            console.log('Order saved:', data);
        })
        .catch(error => {
            console.error('Error saving order:', error);
        });
}