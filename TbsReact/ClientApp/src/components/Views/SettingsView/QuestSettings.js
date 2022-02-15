import {
	Button,
	FormControlLabel,
	Grid,
	Switch,
	Typography,
	MenuItem,
} from "@mui/material";
import React from "react";
import { useForm } from "react-hook-form";

import ContentBox from "../../ContentBox";
import MaterialSelect from "../../ref/MaterialSelect";

const QuestSettings = () => {
	const { register, control, handleSubmit } = useForm();
	const onSubmit = (data) => console.log(data);

	const options = [
		{ value: "1", label: "1" },
		{ value: "2", label: "2" },
		{ value: "3", label: "3" },
	];
	return (
		<>
			<ContentBox>
				<Typography variant="h6">Quest Settings</Typography>
				<form onSubmit={handleSubmit(onSubmit)}>
					<Grid container spacing={3}>
						<Grid item xs={12}>
							<FormControlLabel
								control={<Switch {...register("daily")} />}
								label="Daily quest claim"
							/>
						</Grid>
						<Grid item xs={12}>
							<FormControlLabel
								control={<Switch {...register("begginer")} />}
								label="Beginner quest claim"
							/>
						</Grid>
						<Grid item xs={12}>
							<MaterialSelect
								label="Village claim"
								name="village"
								control={control}
								options={options}
								defaultValue={10}
								fullWidth
							>
								<MenuItem value={10}>Ten</MenuItem>
								<MenuItem value={20}>Twenty</MenuItem>
								<MenuItem value={30}>Thirty</MenuItem>
							</MaterialSelect>
						</Grid>
						<Grid item xs={12}>
							<Button
								type="submit"
								variant="contained"
								value="submit"
								color="success"
							>
								Save
							</Button>
						</Grid>
					</Grid>
				</form>
			</ContentBox>
		</>
	);
};
export default QuestSettings;
