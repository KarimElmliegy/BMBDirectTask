import { api } from "./api.js";
import { state } from "./state.js";
import { elements, escapeHtml, money, showMessage } from "./ui.js";
import { renderProducts } from "./catalog.js";

export function initCart({ onOrderCreated, onError }) {
  elements.cartList.addEventListener("click", event => {
    const id = Number(event.target.dataset.removeVariant);
    if (id) { state.cart = state.cart.filter(item => item.productVariantId !== id); renderCart(); renderProducts(); }
  });
  elements.cartList.addEventListener("change", event => {
    const id = Number(event.target.dataset.quantity);
    const item = state.cart.find(entry => entry.productVariantId === id);
    if (item) { item.quantity = Math.max(1, Math.min(5, item.availableQuantity, Number(event.target.value) || 1)); renderCart(); renderProducts(); }
  });
  elements.orderForm.addEventListener("submit", event => createOrder(event, onOrderCreated, onError));
}

export function addToCart(variantId) {
  const variant = state.products.flatMap(product => product.variants.map(item => ({ ...item, productName: product.name }))).find(item => item.id === variantId);
  const existing = state.cart.find(item => item.productVariantId === variantId);
  const maxQuantity = Math.min(5, variant.quantity);
  if (existing) {
    if (existing.quantity >= maxQuantity) { showMessage(`Only ${maxQuantity} unit(s) can be added for this product.`, true); return; }
    existing.quantity += 1;
  } else if (maxQuantity > 0) {
    state.cart.push({ productVariantId: variant.id, productName: variant.productName, label: `${variant.color} · ${variant.memorySize} / ${variant.storageSize}`, unitPrice: variant.price, availableQuantity: variant.quantity, quantity: 1 });
  } else { showMessage("This product is out of stock.", true); return; }
  renderCart();
  renderProducts();
  showMessage(`${variant.productName} (${variant.color}) was added to your order.`);
}

export function renderCart() {
  const itemCount = state.cart.reduce((total, item) => total + item.quantity, 0);
  elements.cartCount.textContent = itemCount;
  elements.cartCount.setAttribute("aria-label", `${itemCount} item${itemCount === 1 ? "" : "s"} in order`);
  if (!state.cart.length) {
    elements.cartList.innerHTML = '<p class="empty">Add a phone variant to start an order.</p>';
    elements.cartTotal.textContent = money(0);
    return;
  }
  elements.cartList.innerHTML = state.cart.map(item => `<div class="cart-row"><div><strong>${escapeHtml(item.productName)}</strong><small>${escapeHtml(item.label)} · ${money(item.unitPrice)} · Available: ${item.availableQuantity}</small></div><input class="quantity" type="number" min="1" max="${Math.min(5, item.availableQuantity)}" value="${item.quantity}" aria-label="Quantity" data-quantity="${item.productVariantId}"><button class="remove-item" type="button" aria-label="Remove item" title="Remove item" data-remove-variant="${item.productVariantId}">×</button></div>`).join("");
  elements.cartTotal.textContent = money(state.cart.reduce((total, item) => total + item.unitPrice * item.quantity, 0));
}

export function syncCartInventory() {
  const variants = state.products.flatMap(product => product.variants);
  state.cart = state.cart.flatMap(item => {
    const variant = variants.find(entry => entry.id === item.productVariantId);
    if (!variant || variant.quantity < 1) return [];
    item.availableQuantity = variant.quantity;
    item.quantity = Math.min(item.quantity, 5, variant.quantity);
    return [item];
  });
  renderCart();
}

function createOrder(event, onOrderCreated, onError) {
  event.preventDefault();
  if (!state.cart.length) { showMessage("Add at least one phone variant before creating an order.", true); return; }
  const payload = { description: elements.orderDescription.value.trim(), items: state.cart.map(item => ({ productVariantId: item.productVariantId, quantity: item.quantity })) };
  api("/orders", { method: "POST", body: JSON.stringify(payload) }).then(order => {
    state.cart = [];
    elements.orderForm.reset();
    renderCart();
    onOrderCreated(order);
    showMessage("Order created.");
  }).catch(onError);
}
