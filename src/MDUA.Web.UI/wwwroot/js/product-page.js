let currentVariant = null;
let currentPrice = window.productData.basePrice;

// --- Quantity Logic ---
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

// --- UI Helpers ---
function scrollToOrder() {
    document.getElementById("orderSection").scrollIntoView({ behavior: "smooth" });
}

function scrollToTop() {
    window.scrollTo({ top: 0, behavior: "smooth" });
}

function hideReceipt() {
    document.getElementById("receipt").style.display = "none";
}

// --- Variant Selection Logic ---
function findMatchingVariant() {
    const selectors = document.querySelectorAll('.variant-selector');
    const selectedAttributes = {};
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

    if (!allSelected) return null;

    const variants = window.productData.variants;
    const variantAttributeValues = window.productData.variantAttributeValues;

    for (let variant of variants) {
        const variantAttrs = variantAttributeValues.filter(vav => vav.VariantId === variant.Id);
        let matches = true;
        for (let attrId in selectedAttributes) {
            const expectedValueId = selectedAttributes[attrId];
            const hasMatch = variantAttrs.some(vav =>
                vav.AttributeId === parseInt(attrId) && vav.AttributeValueId === expectedValueId
            );
            if (!hasMatch) { matches = false; break; }
        }
        if (matches && variantAttrs.length === Object.keys(selectedAttributes).length) return variant;
    }
    return null;
}

function updateVariantSelection() {
    const variant = findMatchingVariant();
    const imgContainer = document.getElementById("variant-image-container");
    const imgElement = document.getElementById("selected-variant-img");

    if (variant) {
        currentVariant = variant;

        // Price
        const priceStock = window.productData.variantPriceStocks.find(vps => vps.VariantId === variant.Id);
        currentPrice = priceStock ? (priceStock.Price || window.productData.basePrice) : (variant.VariantPrice || window.productData.basePrice);

        // UI Update
        document.getElementById("selectedVariantId").value = variant.Id;
        document.getElementById("selectedVariantPrice").value = currentPrice;
        document.getElementById("displayPrice").textContent = `${currentPrice}`;
        document.getElementById("selectedVariantName").textContent = variant.VariantName;

        // Thumbnail
        const variantImages = window.productData.variantImages[variant.Id];
        if (variantImages && variantImages.length > 0) {
            imgElement.src = `/images/products/${variantImages[0].ImageUrl}`;
            imgContainer.style.display = "block";
        } else {
            imgContainer.style.display = "none";
        }

        updateTotal();
        showVariantAvailability(priceStock);
    } else {
        // Reset
        currentPrice = window.productData.basePrice;
        document.getElementById("selectedVariantId").value = "";
        document.getElementById("displayPrice").textContent = `${currentPrice}`;
        document.getElementById("selectedVariantName").textContent = window.productData.productName;
        imgContainer.style.display = "none";

        updateTotal();

        // Check incomplete selection
        let hasSelection = false;
        document.querySelectorAll('.variant-selector').forEach(s => { if (s.value) hasSelection = true; });
        if (hasSelection && !variant) showVariantUnavailable();
    }
}
document.getElementById("phone").addEventListener("blur", function () {
    const phone = this.value.trim();

    // Only check if phone is valid length (e.g., 11 digits for BD)
    if (phone.length >= 11) {
        // Optional: visual indicator
        // document.getElementById("name").setAttribute("placeholder", "Searching...");

        fetch(`/product/check-customer/${phone}`)
            .then(res => res.json())
            .then(data => {
                if (data.found) {
                    // Fill Name
                    if (data.name) document.getElementById("name").value = data.name;

                    // Fill Email (ignore dummy guest emails)
                    if (data.email && !data.email.includes("@guest.voot.com") && !data.email.includes("noemail")) {
                        document.getElementById("email").value = data.email;
                    }

                    // Fill Address
                    if (data.lastAddress) {
                        const addr = data.lastAddress;
                        if (addr.street) document.getElementById("street").value = addr.street;
                        if (addr.city) document.getElementById("city").value = addr.city;
                        if (addr.divison) document.getElementById("division").value = addr.divison;
                        if (addr.postalCode) document.getElementById("postalCode").value = addr.postalCode;
                        if (addr.zipCode) document.getElementById("zipCode").value = addr.zipCode;
                        if (addr.addressType) document.getElementById("addressType").value = addr.addressType;
                    }
                }
            })
            .catch(err => console.log("Customer check error:", err));
    }
});
function showVariantAvailability(priceStock) {
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

// --- Modal Logic ---
function openImageModal() {
    const modal = document.getElementById("imageModal");
    const img = document.getElementById("selected-variant-img");
    const modalImg = document.getElementById("modal-img");
    const captionText = document.getElementById("modal-caption");

    modal.style.display = "block";
    modalImg.src = img.src;
    captionText.innerHTML = document.getElementById("selectedVariantName").textContent;
}

function closeImageModal() {
    document.getElementById("imageModal").style.display = "none";
}

document.addEventListener('keydown', function (event) {
    if (event.key === "Escape") closeImageModal();
});

// --- SUBMIT ORDER ---
function submitOrder() {
    // 1. Collect Info
    const name = document.getElementById("name").value.trim();
    const phone = document.getElementById("phone").value.trim();
    const email = document.getElementById("email").value.trim(); // New Field
    const street = document.getElementById("street").value.trim();
    const city = document.getElementById("city").value.trim();
    const division = document.getElementById("division").value;
    const postalCode = document.getElementById("postalCode").value.trim();
    const zipCode = document.getElementById("zipCode").value.trim(); // New Field
    const addressType = document.getElementById("addressType").value;
    const country = document.getElementById("country").value;
    const quantity = parseInt(document.getElementById("quantity").value);
    const variantId = document.getElementById("selectedVariantId").value;

    // 2. Validation
    if (!name || !phone) {
        alert("❌ নাম এবং মোবাইল নাম্বার আবশ্যক।");
        return;
    }
    if (!/^\d{11}$/.test(phone)) {
        alert("❌ ১১ সংখ্যার সঠিক মোবাইল নাম্বার দিন।");
        return;
    }
    if (!street || !city || !division || !postalCode || !zipCode) {
        alert("❌ ঠিকানার সকল তথ্য (পোস্ট কোড এবং জিপ কোড সহ) পূরণ করুন।");
        return;
    }
    if (!variantId) {
        alert("❌ অনুগ্রহ করে সাইজ/কালার নির্বাচন করুন।");
        return;
    }

    // 3. Stock Check (Optional - Facade also checks)
    const priceStock = window.productData.variantPriceStocks.find(vps => vps.VariantId === parseInt(variantId));
    if (priceStock && priceStock.TrackInventory && priceStock.StockQty < quantity && !priceStock.AllowBackorder) {
        alert(`❌ দুঃখিত, স্টকে মাত্র ${priceStock.StockQty} টি আছে।`);
        return;
    }

    // 4. Prepare Payload
    const total = (currentPrice * quantity) + (currentPrice * quantity < 1999 ? 100 : 0);
    const orderPayload = {
        Customer: {
            Name: name,
            Phone: phone,
            Email: email
        },
        Address: {
            Street: street,
            City: city,
            Divison: division,
            PostalCode: postalCode,
            ZipCode: zipCode,
            Country: country,
            AddressType: addressType
        },
        Order: {
            VariantId: parseInt(variantId),
            Quantity: quantity,
            TotalAmount: total
        }
    };

    // 5. Send Request
    const submitBtn = document.querySelector('.submit-order-btn');
    const originalBtnText = submitBtn.innerHTML;
    submitBtn.disabled = true;
    submitBtn.innerHTML = '⏳ প্রসেসিং হচ্ছে...';

    fetch('/product/place-order', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(orderPayload)
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Populate Receipt
                document.getElementById("receipt-subtotal").textContent = `৳ ${currentPrice * quantity}`;
                document.getElementById("delivery-charge").textContent = `৳ ${total - (currentPrice * quantity)}`;
                document.getElementById("receipt-total").textContent = `৳ ${total}`;
                document.getElementById("receipt-product").textContent = currentVariant ? currentVariant.VariantName : "Selected Item";
                document.getElementById("receipt-name").textContent = name;
                document.getElementById("receipt-phone").textContent = phone;
                document.getElementById("receipt-address").textContent = `${street}, ${city} - ${postalCode} (${addressType})`;
                document.getElementById("receipt-location").textContent = division;
                document.getElementById("receipt-quantity").textContent = quantity;

                // Show Receipt
                const receipt = document.getElementById("receipt");
                receipt.style.display = "block";
                receipt.scrollIntoView({ behavior: "smooth" });

                alert(data.message);
            } else {
                alert("❌ " + data.message);
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert("❌ সিস্টেম এরর: অর্ডার সম্পন্ন করা যায়নি।");
        })
        .finally(() => {
            submitBtn.disabled = false;
            submitBtn.innerHTML = originalBtnText;
        });
}

// --- Initialization ---
window.addEventListener("scroll", () => {
    const scrollTop = document.getElementById("scrollTop");
    scrollTop.classList.toggle("visible", window.scrollY > 300);
});

window.addEventListener('DOMContentLoaded', () => {
    updateTotal();

    const selectors = document.querySelectorAll('.variant-selector');
    selectors.forEach(selector => {
        selector.addEventListener('change', () => {
            updateVariantSelection();
            hideReceipt();
        });
    });

    const orderFormElements = document.querySelectorAll('.order-form input, .order-form textarea, .order-form select');
    orderFormElements.forEach(el => {
        el.addEventListener('input', hideReceipt);
        el.addEventListener('change', hideReceipt);
    });
});

// --- POSTAL CODE LOGIC ---
let postalData = {};

// 1. Fetch and Parse JSON Data
fetch('/data/bd-postcodes.json') // Ensure this path matches where you saved the file
    .then(response => response.json())
    .then(data => {
        // Process data to handle keys with spaces like "1206 " -> "1206"
        // and flatten structure if needed (e.g., if your file has "en" inside)
        for (let key in data) {
            let cleanKey = key.trim();
            // Check if structure is direct or nested under 'en'
            let info = data[key].en ? data[key].en : data[key];

            postalData[cleanKey] = {
                division: info.division ? info.division.trim() : "",
                district: info.district ? info.district.trim() : ""
            };
        }
        console.log("Postal Codes Loaded:", Object.keys(postalData).length);
    })
    .catch(err => console.error("Error loading postal codes:", err));

// 2. Input Listener
function handlePostalCodeInput(e) {
    const code = e.target.value.trim();

    // If we have data for this code
    if (postalData[code]) {
        const info = postalData[code];

        // Auto-fill City (District)
        const cityInput = document.getElementById("city");
        if (cityInput) {
            cityInput.value = info.district;
            // Visual cue
            cityInput.style.backgroundColor = "#e8f0fe";
            setTimeout(() => cityInput.style.backgroundColor = "", 500);
        }

        // Auto-select Division
        const divSelect = document.getElementById("division");
        if (divSelect) {
            // Create a mapping if spelling differs (e.g., 'Chattogram' vs 'Chittagong')
            // But based on your provided JSON, they seem to match your HTML values.
            divSelect.value = info.division;
        }

        // Sync Zip Code field if it exists and is empty
        const zipInput = document.getElementById("zipCode");
        if (zipInput && !zipInput.value) {
            zipInput.value = code;
        }
    }
}

// 3. Attach to DOM
document.addEventListener('DOMContentLoaded', () => {
    const postalInput = document.getElementById("postalCode");
    if (postalInput) {
        postalInput.addEventListener("input", handlePostalCodeInput);
    }

    // Also attach to ZipCode just in case user types there
    const zipInput = document.getElementById("zipCode");
    if (zipInput) {
        zipInput.addEventListener("input", handlePostalCodeInput);
    }
});