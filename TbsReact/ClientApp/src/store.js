/* import { configureStore } from '@reduxjs/toolkit'
import accountReducer from './slices/account'
export default configureStore({
  reducer: {
    account: accountReducer,
  },
  devTools: process.env.NODE_ENV !== 'production',
}) */

import { configureStore } from "@reduxjs/toolkit";

import storage from "redux-persist/lib/storage";
import { combineReducers } from "redux";
import { persistReducer } from "redux-persist";
import autoMergeLevel2 from "redux-persist/lib/stateReconciler/autoMergeLevel2";

import thunk from "redux-thunk";

import accountReducer from "./slices/account";

const reducers = combineReducers({
	account: accountReducer,
});
const persistConfig = {
	key: "tbsroot",
	storage,
	stateReconciler: autoMergeLevel2,
};

const persistedReducer = persistReducer(persistConfig, reducers);

export default configureStore({
	reducer: persistedReducer,
	devTools: process.env.NODE_ENV !== "production",
	middleware: [thunk],
});
