import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import APICall from "./ApiRequest";

export const api = createApi({
  baseQuery: fetchBaseQuery({ baseUrl: "https://localhost:5001/api" }),
  reducerPath: "adminApi",
  tagTypes: ["Auth", "User", "Products", "Customers", "Orders", "Reports"],
  endpoints: (build) => ({
    login: build.mutation({
      queryFn: async (credentials) => {
        try {
          const response = await APICall("POST", "/Login/login", {
            body: credentials,
          });
          return { data: response.data };
        } catch (error) {
          return { error: error.message };
        }
      },
      invalidatesTags: ["Auth"],
    }),
    getUser: build.query({
      queryFn: async (id) => {
        try {
          const response = await APICall("GET", `/User/${id}`);
          return { data: response.data };
        } catch (error) {
          return { error: error.message };
        }
      },
      providesTags: ["User"],
    }),
    getProducts: build.query({
      queryFn: async () => {
        try {
          const response = await APICall("GET", "/Product/GetAll");
          return { data: response.data };
        } catch (error) {
          return { error: error.message };
        }
      },
      providesTags: ["Products"],
    }),
    getCustomers: build.query({
      queryFn: async () => {
        try {
          const response = await APICall("GET", "client/customers");
          return { data: response.data };
        } catch (error) {
          return { error: error.message };
        }
      },
      providesTags: ["Customers"],
    }),
    getTransactions: build.query({
      queryFn: async ({ page, pageSize, sort, search }) => {
        try {
          const response = await APICall("GET", "client/transactions", {
            params: { page, pageSize, sort, search },
          });
          return { data: response.data };
        } catch (error) {
          return { error: error.message };
        }
      },
      providesTags: ["Transactions"],
    }),
    getGeography: build.query({
      queryFn: async () => {
        try {
          const response = await APICall("GET", "client/geography");
          return { data: response.data };
        } catch (error) {
          return { error: error.message };
        }
      },
      providesTags: ["Geography"],
    }),
    getSales: build.query({
      queryFn: async () => {
        try {
          const response = await APICall("GET", "sales/sales");
          return { data: response.data };
        } catch (error) {
          return { error: error.message };
        }
      },
      providesTags: ["Sales"],
    }),
    getAdmins: build.query({
      queryFn: async () => {
        try {
          const response = await APICall("GET", "management/admins");
          return { data: response.data };
        } catch (error) {
          return { error: error.message };
        }
      },
      providesTags: ["Admins"],
    }),
    getUserPerformance: build.query({
      queryFn: async (id) => {
        try {
          const response = await APICall("GET", `management/performance/${id}`);
          return { data: response.data };
        } catch (error) {
          return { error: error.message };
        }
      },
      providesTags: ["Performance"],
    }),
    getDashboard: build.query({
      queryFn: async () => {
        try {
          const response = await APICall("GET", "general/dashboard");
          return { data: response.data };
        } catch (error) {
          return { error: error.message };
        }
      },
      providesTags: ["Dashboard"],
    }),
    getOrders: build.query({
      queryFn: async ({ page, pageSize }) => {
        try {
          const response = await APICall("GET", "/Order/GetAll", {
            params: { page, pageSize },
          });
          return { data: response.data };
        } catch (error) {
          return { error: error.message };
        }
      },
      providesTags: ["Orders"],
    }),
    getSalesReport: build.query({
      queryFn: async ({ startDate, endDate }) => {
        try {
          const response = await APICall("GET", "/Report/sales", {
            params: { startDate, endDate },
          });
          return { data: response.data };
        } catch (error) {
          return { error: error.message };
        }
      },
      providesTags: ["Reports"],
    }),
  }),
});

export const {
  useLoginMutation,
  useGetUserQuery,
  useGetProductsQuery,
  useGetCustomersQuery,
  useGetTransactionsQuery,
  useGetGeographyQuery,
  useGetSalesQuery,
  useGetAdminsQuery,
  useGetUserPerformanceQuery,
  useGetDashboardQuery,
  useGetOrdersQuery,
  useGetSalesReportQuery,
} = api;
