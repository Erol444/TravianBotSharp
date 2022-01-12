import React from "react";
import { Grid, Box, Typography } from "@mui/material";
const style = {
	position: "absolute",
	top: "50%",
	left: "50%",
	transform: "translate(-50%, -50%)",
	bgcolor: "background.paper",
	border: "2px solid #000",
	boxShadow: 24,
	p: 4,
	width: "80%",
	height: "80%",
};

const Info = () => {
	return (
		<>
			<Box sx={style} justifyContent="center" alignItems="center">
				<Grid spacing={2} container direction="column">
					<Grid item xs={12}>
						<Typography
							variant="h4"
							noWrap
							component="div"
							sx={{ mr: 2, display: { xs: "none", md: "flex" } }}
						>
							TravianBotSharp
						</Typography>
					</Grid>
					<Grid item xs={12}>
						Join our Discord Server
						<br />
						<a
							href="https://discord.gg/mBa4f2K"
							target="_blank"
							rel="noreferrer"
						>
							<img
								alt="TravianBotSharp Discord server"
								src="https://discordapp.com/api/guilds/740829801446637601/widget.png?style=banner2"
							></img>
						</a>
					</Grid>
					<Grid item xs={12}>
						If you would like to make a donation, here it is <br />
						<a
							href="https://ko-fi.com/T6T3648VG"
							target="_blank"
							rel="noreferrer"
						>
							<img
								height="36"
								style={{ border: "0px", height: "36px" }}
								src="https://cdn.ko-fi.com/cdn/kofi3.png?v=3"
								border="0"
								alt="Buy Me a Coffee at ko-fi.com"
							/>
						</a>
					</Grid>
					<Grid item xs={12}>
						In case you are Vietnamese, đây là link cho Momo ._.{" "}
						<br />
						<a
							href="https://nhantien.momo.vn/8C4j4DfmHO3"
							target="_blank"
							rel="noreferrer"
						>
							<img
								height="36"
								style={{ border: "0px", height: "36px" }}
								src="https://upload.wikimedia.org/wikipedia/vi/f/fe/MoMo_Logo.png"
								border="0"
								alt="Buy Me a Coffee at ko-fi.com"
							/>
						</a>
					</Grid>
				</Grid>
			</Box>
		</>
	);
};

export default Info;
