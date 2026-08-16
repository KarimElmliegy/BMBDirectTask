import { api } from "./api.js";
import { state } from "./state.js";
import { elements, escapeHtml, money } from "./ui.js";

const imageExtensions = ["jpg", "jpeg", "png", "webp"];

export function initCatalog({ onAddVariant }) {
  elements.productsList.addEventListener("click", event => {
    const variantButton = event.target.closest("[data-variant-direction]");
    if (variantButton) {
      const product = state.products.find(item => item.id === Number(variantButton.dataset.productId));
      if (!product) return;
      const currentIndex = state.variantIndexes[product.id] || 0;
      state.variantIndexes[product.id] = Math.max(0, Math.min(product.variants.length - 1, currentIndex + Number(variantButton.dataset.variantDirection)));
      renderProducts();
      return;
    }
    const id = Number(event.target.closest("[data-add-variant]")?.dataset.addVariant);
    if (id) onAddVariant(id);
  });
  elements.productsList.addEventListener("error", showNextProductImage, true);
  elements.productsPrevious.addEventListener("click", () => moveProducts(-1));
  elements.productsNext.addEventListener("click", () => moveProducts(1));
  elements.productsPagination.addEventListener("click", event => {
    const page = Number(event.target.dataset.productPage);
    if (Number.isInteger(page)) { state.productPage = page; renderProducts(); }
  });
}

export function loadProducts() {
  return api("/products").then(products => { state.products = products; renderProducts(); });
}

export function renderProducts() {
  if (!state.products.length) {
    elements.productsList.innerHTML = '<p class="empty">No products are available yet.</p>';
    updateCarouselControls();
    return;
  }

  const pageSize = 6;
  const pageCount = Math.ceil(state.products.length / pageSize);
  state.productPage = Math.min(state.productPage, pageCount - 1);
  const firstProductIndex = state.productPage * pageSize;
  const accountBanned = Boolean(state.session?.bannedUntil && new Date(state.session.bannedUntil) > new Date());
  elements.productsList.innerHTML = state.products.slice(firstProductIndex, firstProductIndex + pageSize).map((product, index) => {
    const selectedVariantIndex = Math.min(state.variantIndexes[product.id] || 0, product.variants.length - 1);
    const variant = product.variants[selectedVariantIndex];
    const addedItem = state.cart.find(item => item.productVariantId === variant.id);
    const maxQuantity = Math.min(5, variant.quantity);
    return `<article class="product">
      <img class="product-image" src="Phonesimages/phone${firstProductIndex + index + 1}.jpg" alt="${escapeHtml(product.name)}" data-image-extension="0" data-image-number="${firstProductIndex + index + 1}">
      <div class="product-content"><div class="product-header"><div><h3>${escapeHtml(product.name)}</h3><p>${escapeHtml(product.description)}</p></div></div>
      <div class="variant-list"><div class="variant"><div class="variant-navigation"><button class="variant-arrow" type="button" data-variant-direction="-1" data-product-id="${product.id}" aria-label="Previous ${escapeHtml(product.name)} variant" ${selectedVariantIndex === 0 ? "disabled" : ""}>‹</button><span>Variant ${selectedVariantIndex + 1} of ${product.variants.length}</span><button class="variant-arrow" type="button" data-variant-direction="1" data-product-id="${product.id}" aria-label="Next ${escapeHtml(product.name)} variant" ${selectedVariantIndex === product.variants.length - 1 ? "disabled" : ""}>›</button></div><div class="variant-info"><strong>${escapeHtml(variant.color)}</strong><div class="variant-details">Memory: ${escapeHtml(variant.memorySize)} · Storage: ${escapeHtml(variant.storageSize)} · Available: ${variant.quantity}${variant.otherDetails ? ` · ${escapeHtml(variant.otherDetails)}` : ""}</div></div><div class="variant-action"><span class="price">${money(variant.price)}</span><button class="button button-secondary" type="button" data-add-variant="${variant.id}" ${accountBanned || maxQuantity === 0 || addedItem?.quantity >= maxQuantity ? "disabled" : ""}>${accountBanned ? "Temporarily unavailable" : maxQuantity === 0 ? "Out of stock" : addedItem?.quantity >= maxQuantity ? "Maximum added" : "Add to order"}</button></div>${addedItem ? `<p class="selected-variant">Added to order: ${escapeHtml(variant.color)} · ${escapeHtml(variant.memorySize)} / ${escapeHtml(variant.storageSize)} · Quantity: ${addedItem.quantity}</p>` : ""}</div></div></div>
    </article>`;
  }).join("");
  elements.productsPagination.innerHTML = Array.from({ length: pageCount }, (_, index) => `<button class="carousel-page-button${index === state.productPage ? " is-active" : ""}" type="button" data-product-page="${index}" aria-label="Show product page ${index + 1}" aria-current="${index === state.productPage ? "true" : "false"}">${index + 1}</button>`).join("");
  updateCarouselControls();
}

function moveProducts(direction) { state.productPage += direction; renderProducts(); }
function updateCarouselControls() {
  const pageCount = Math.ceil(state.products.length / 6);
  elements.productsPrevious.disabled = state.productPage <= 0;
  elements.productsNext.disabled = state.productPage >= pageCount - 1;
}
function showNextProductImage(event) {
  const image = event.target;
  const extensionIndex = Number(image.dataset.imageExtension) + 1;
  if (extensionIndex < imageExtensions.length) {
    image.dataset.imageExtension = extensionIndex;
    image.src = `Phonesimages/phone${image.dataset.imageNumber}.${imageExtensions[extensionIndex]}`;
  } else image.hidden = true;
}
