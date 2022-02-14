import {
	Button,
	FormControlLabel,
	Grid,
	Switch,
	TextField,
	Typography,
} from "@mui/material";
import React, { useEffect, useState } from "react";

import ContentBox from "../../ContentBox";

const DiscordSettings = () => {
	const [value, setValue] = useState(false);
	useEffect(() => {
		console.log("value", value);
	}, [value]);
	return (
		<>
			<ContentBox>
				<Grid container spacing={3}>
					<Grid item xs={12}>
						<Typography variant="h6">Discord Settings</Typography>
					</Grid>
					<Grid item xs={12}>
						<Grid container spacing={3}>
							<Grid item xs={12}>
								<FormControlLabel
									control={
										<Switch
											checked={value}
											onChange={(event) => {
												setValue(event.target.checked);
											}}
										/>
									}
									label="Active"
								/>
							</Grid>
							<Grid item xs={12}>
								<FormControlLabel
									control={
										<Switch
											checked={value}
											onChange={(event) => {
												setValue(event.target.checked);
											}}
										/>
									}
									label="Send message when account online"
								/>
							</Grid>
							<Grid item xs={8}>
								<TextField
									fullWidth
									label="URL webhook"
									variant="outlined"
								/>
							</Grid>
							<Grid item xs={4}>
								<Button variant="contained">Check</Button>
							</Grid>
						</Grid>
					</Grid>
					<Grid item xs={12}>
						<Button variant="contained" color="success">
							Save
						</Button>
					</Grid>
				</Grid>
			</ContentBox>
		</>
	);
};
export default DiscordSettings;
