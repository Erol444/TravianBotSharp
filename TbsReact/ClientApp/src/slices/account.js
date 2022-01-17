import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";

import { getAccount } from "../api/Accounts/Account";
export const fetchAccountByID = createAsyncThunk(
	"account/fetchById",
	async (id, thunkAPI) => {
		const data = await getAccount(id);
		return data;
	}
);

const initialState = {
	id: -1,
	name: "Not selected",
	serverUrl: "",
};

export const accountSlice = createSlice({
	name: "account",
	initialState,
	reducers: {
		setAccount: (state, action) => {
			return action.payload;
		},

		resetAccount: (state, action) => {
			return initialState;
		},
	},
	extraReducers: (builder) => {
		builder.addCase(fetchAccountByID.fulfilled, (state, action) => {
			return action.payload;
		});
	},
});

export const { setAccount, resetAccount } = accountSlice.actions;
export default accountSlice.reducer;
