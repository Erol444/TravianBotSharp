import {
	Button,
	FormControl,
	FormControlLabel,
	FormHelperText,
	Grid,
	InputLabel,
	MenuItem,
	Select,
	Switch,
	Typography,
} from "@mui/material";
import React, { useEffect, useState } from "react";

import ContentBox from "../../ContentBox";

const QuestSettings = () => {
	const [value, setValue] = useState(false);
	useEffect(() => {
		console.log("value", value);
	}, [value]);
	return (
		<>
			<ContentBox>
				<Grid container spacing={3}>
					<Grid item xs={12}>
						<Typography variant="h6">Quest Settings</Typography>
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
									label="Daily quest claim"
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
									label="Beginner quest claim"
								/>
							</Grid>
							<Grid item xs={12}>
								<FormControl>
									<InputLabel>Village claim</InputLabel>

									<Select label="Village claim">
										<MenuItem value={10}>Ten</MenuItem>
										<MenuItem value={20}>Twenty</MenuItem>
										<MenuItem value={30}>Thirty</MenuItem>
									</Select>
									<FormHelperText>
										Only for privated server
									</FormHelperText>
								</FormControl>
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
export default QuestSettings;
