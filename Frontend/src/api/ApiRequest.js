import axios from "axios";

/**
 * Makes an HTTP request using axios
 * @param {string} method - The HTTP method (GET, POST, PUT, DELETE, etc.)
 * @param {string} url - The endpoint URL
 * @param {Object} data - Request configuration
 * @param {Object} [data.params] - URL parameters for GET requests
 * @param {Object} [data.headers] - Custom headers
 * @param {any} [data.body] - Request body for non-GET requests
 * @returns {Promise} Axios response promise
 */
export default function APICall(method, url, data = {}) {
  const baseURL =
    process.env.REACT_APP_BASE_URL ||
    "https://localhost:5001/swagger/index.html";
  const fullUrl = `${baseURL}/${url}`
    .replace(/\/+/g, "/") // Remove duplicate slashes
    .replace(/\/$/, ""); // Remove trailing slash

  const config = {
    method: method.toUpperCase(),
    url: fullUrl,
    headers: {
      "Content-Type": "application/json",
      ...data.headers,
    },
  };

  // Handle GET requests differently than other methods
  if (config.method === "GET") {
    config.params = data.params || {};
  } else {
    config.data = data.body || data;
  }

  return axios(config).catch((error) => {
    // Log error details for debugging
    console.error("API Request Failed:", {
      method: config.method,
      url: fullUrl,
      error: error.message,
    });
    throw error;
  });
}
