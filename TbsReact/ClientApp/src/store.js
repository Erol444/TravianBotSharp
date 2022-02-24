import { configureStore } from "@reduxjs/toolkit";

import accountReducer from "./slices/account";
import villageReducer from "./slices/village";

export default configureStore({
	reducer: {
		account: accountReducer,
		village: villageReducer,
	},
});
