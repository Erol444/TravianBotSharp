import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";

import { getAccount } from "../api/Accounts/Account";
import { getStatus } from "../api/Accounts/Driver";
export const fetchAccountByID = createAsyncThunk(
	"account/fetcInfohById",
	async (id, thunkAPI) => {
		const data = await getAccount(id);
		return data;
	}
);

export const fetchStatusByID = createAsyncThunk(
	"account/fetchStatusById",
	async (id, thunkAPI) => {
		const data = await getStatus(id);
		return data;
	}
);

const initialState = {
	info: {
		id: -1,
		name: "Not selected",
		serverUrl: "",
	},
	status: false,
};

export const accountSlice = createSlice({
	name: "account",
	initialState,
	reducers: {
		setAccount: (state, action) => {
			state.info = action.payload;
		},

		resetAccount: (state, action) => {
			return initialState;
		},

		setStatus: (state, action) => {
			state.status = action.payload;
		},
	},
	extraReducers: (builder) => {
		builder.addCase(fetchAccountByID.fulfilled, (state, action) => {
			state.info = action.payload;
		});
		builder.addCase(fetchStatusByID.fulfilled, (state, action) => {
			state.status = action.payload;
		});
	},
});

export const { setAccount, resetAccount } = accountSlice.actions;
export default accountSlice.reducer;
