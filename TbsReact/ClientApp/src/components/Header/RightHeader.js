import React from "react";
import { Box, Chip } from "@mui/material";
import { AccountCircle } from "@mui/icons-material";
import { useSelector } from "react-redux";

const RightHeader = () => {
	const account = useSelector((state) => state.account);
	return (
		<>
			<Box sx={{ display: "flex" }}>
				<Chip
					icon={<AccountCircle />}
					label={account.info.name}
					color="success"
				/>
				{account.info.id !== -1 && (
					<>
						<Chip label={account.info.serverUrl} color="warning" />
						<Chip
							label={account.status ? "ACTIVE" : "INACTIVE"}
							color={account.status ? "success" : "error"}
						/>
					</>
				)}
			</Box>
		</>
	);
};

export default RightHeader;
